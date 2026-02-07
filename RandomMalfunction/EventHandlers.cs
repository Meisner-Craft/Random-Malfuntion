using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using FLXLib.Extensions;
using LightContainmentZoneDecontamination;
using MEC;
using System.Collections.Generic;
using System.Linq;

namespace RandomMalfunction
{
    public class EventHandlers
    {
        private float secondsLeft = 0;

        public void OnRoundStarted()
        {
            // Блок всех КПП в ЛКЗ
            if (PluginMain.Singleton.Config.ChanceList.ContainsKey("BlockAllLczCheckpoint"))
            {
                PluginMain.Singleton.Config.ChanceList.TryGetValue("BlockAllLczCheckpoint", out byte chance);
                if (CommonExtensions.ChanceChecker(chance))
                {
                    var cassie = PluginMain.Singleton.Config.BlockLczCheckpointsCassie;
                    Cassie.MessageTranslated(cassie.Cassie, cassie.Translation);

                    Timing.CallDelayed(PluginMain.Singleton.Config.DelayBeforeEvents, BlockLcsChekpoints);
                }
            }


            // Открытие всех дверей
            if (PluginMain.Singleton.Config.ChanceList.ContainsKey("OpenAllDoors"))
            {
                PluginMain.Singleton.Config.ChanceList.TryGetValue("OpenAllDoors", out byte chance);
                if (CommonExtensions.ChanceChecker(chance))
                {
                    var cassie = PluginMain.Singleton.Config.DoorMalfunctionCassie;
                    Cassie.MessageTranslated(cassie.Cassie, cassie.Translation);

                    Timing.CallDelayed(PluginMain.Singleton.Config.DelayBeforeEvents, OpenAllDoors);
                }
            }


            // Поломка света
            if (PluginMain.Singleton.Config.ChanceList.ContainsKey("LightMalfunction"))
            {
                PluginMain.Singleton.Config.ChanceList.TryGetValue("LightMalfunction", out byte chance);
                if (CommonExtensions.ChanceChecker(chance))
                {
                    var cassie = PluginMain.Singleton.Config.LightMalfunctionCassie;
                    Cassie.MessageTranslated(cassie.Cassie, cassie.Translation);

                    Timing.CallDelayed(PluginMain.Singleton.Config.DelayBeforeEvents, LightingMalfunction);
                }
            }


            // Обеззараживание ТЗС
            if (PluginMain.Singleton.Config.ChanceList.ContainsKey("HczDecontamination"))
            {
                PluginMain.Singleton.Config.ChanceList.TryGetValue("HczDecontamination", out byte chance);
                if (CommonExtensions.ChanceChecker(chance))
                {
                    secondsLeft = PluginMain.Singleton.Config.HczDecontaminationDuration;
                    Timing.RunCoroutine(HczDecontamination());
                }
            }


            // Обеззараживание ЛЗС через 10 минут
            if (PluginMain.Singleton.Config.ChanceList.ContainsKey("LczDecontamination10Min"))
            {
                PluginMain.Singleton.Config.ChanceList.TryGetValue("LczDecontamination10Min", out byte chance);
                if (CommonExtensions.ChanceChecker(chance))
                {
                    DecontaminationController.Singleton.TimeOffset = 300f;
                }
            }


            // Обеззараживание ЛЗС через 5 минут
            if (PluginMain.Singleton.Config.ChanceList.ContainsKey("LczDecontamination5Min"))
            {
                PluginMain.Singleton.Config.ChanceList.TryGetValue("LczDecontamination5Min", out byte chance);
                if (CommonExtensions.ChanceChecker(chance))
                {
                    DecontaminationController.Singleton.TimeOffset = 600f;
                }
            }
        }

        public void OnRoundEnded()
        {
            Timing.KillCoroutines();
        }



        private void OpenAllDoors()
        {
            foreach (var door in Exiled.API.Features.Doors.Door.List)
            {
                if (!PluginMain.Singleton.Config.OpenDoorBlackList.Contains(door.Type))
                {
                    door.IsOpen = true;
                }
            }
        }

        private void BlockLcsChekpoints()
        {
            // Кейси

            Door.List.FirstOrDefault((Door d) => d.Type == DoorType.CheckpointLczA).Lock(
                PluginMain.Singleton.Config.LczCheckpointBlockDuration, DoorLockType.Isolation);

            Door.List.FirstOrDefault((Door d) => d.Type == DoorType.CheckpointLczB).Lock(
                PluginMain.Singleton.Config.LczCheckpointBlockDuration, DoorLockType.Isolation);
        }

        private void LightingMalfunction()
        {
            // Получаем данные из конфига
            float duration = PluginMain.Singleton.Config.LightingMalfunctionDuration;
            float periodicity = PluginMain.Singleton.Config.LightingMalfunctionPeriodicity;

            // Шаг в n секунд, пока меньше длительности поломки
            for (float i = 0; i < duration; i += periodicity)
            {
                Exiled.API.Features.Map.TurnOffAllLights(periodicity / 2);

                Timing.WaitForSeconds(periodicity);
            }
        }

        private IEnumerator<float> HczDecontamination()
        {
            // Получаем стадию по секундам из конфига
            if (PluginMain.Singleton.Config.CassieBySeconds.TryGetValue((int)secondsLeft, out var cassie))
            {
                Cassie.MessageTranslated(cassie.Cassie,cassie.Translation);
            }

            if (secondsLeft == 0)
            {
                // Открытие и блок всех дверей в ТЗС
                foreach (Door door in Door.List)
                {
                    if (door.Zone == ZoneType.HeavyContainment &&
                        !PluginMain.Singleton.Config.OpenDoorDecontaminationBlackList.Contains(door.Type))
                    {
                        door.IsOpen = false;
                        door.Lock(float.PositiveInfinity, DoorLockType.DecontLockdown);
                    }
                }

                // Выдача игрокам эффекта обеззараживания
                foreach (var player in Exiled.API.Features.Player.List)
                {
                    if (player.Zone == ZoneType.HeavyContainment)
                    {
                        player.EnableEffect(EffectType.Decontaminating);
                    }
                }

                yield break; // Выход из корутины
            }

            secondsLeft--;

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
