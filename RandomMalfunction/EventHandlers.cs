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
        // Ключи для получения шансов из конфига
        private const string BlockAllLczCheckpointKey = "BlockAllLczCheckpoint";
        private const string OpenAllDoorsKey = "OpenAllDoors";
        private const string LightMalfunctionKey = "LightMalfunction";
        private const string HczDecontaminationKey = "HczDecontamination";
        private const string LczDecontamination10MinKey = "LczDecontamination10Min";
        private const string LczDecontamination5MinKey = "LczDecontamination5Min";

        private float secondsLeft = 0;

        // Массив корутин
        private List<CoroutineHandle> _coroutines = new List<CoroutineHandle>();

        public void OnRoundStarted()
        {
            // Блок всех КПП в ЛКЗ
            if (PluginMain.Singleton.Config.ChanceList.TryGetValue(BlockAllLczCheckpointKey, out byte chance1)
            && CommonExtensions.ChanceChecker(chance1))
            {
                var cassie = PluginMain.Singleton.Config.BlockLczCheckpointsCassie;
                Exiled.API.Features.Cassie.MessageTranslated(cassie.Cassie, cassie.Translation);

                Timing.CallDelayed(PluginMain.Singleton.Config.DelayBeforeEvents, BlockLcsChekpoints);
            }

            // Открытие всех дверей
            if (PluginMain.Singleton.Config.ChanceList.TryGetValue(OpenAllDoorsKey, out byte chance2)
            && CommonExtensions.ChanceChecker(chance2))
            {
                var cassie = PluginMain.Singleton.Config.DoorMalfunctionCassie;
                Exiled.API.Features.Cassie.MessageTranslated(cassie.Cassie, cassie.Translation);

                Timing.CallDelayed(PluginMain.Singleton.Config.DelayBeforeEvents, OpenAllDoors);
            }


            // Поломка света
            if (PluginMain.Singleton.Config.ChanceList.TryGetValue(LightMalfunctionKey, out byte chance3)
            && CommonExtensions.ChanceChecker(chance3))
            {
                var cassie = PluginMain.Singleton.Config.LightMalfunctionCassie;
                Exiled.API.Features.Cassie.MessageTranslated(cassie.Cassie, cassie.Translation);

                Timing.CallDelayed(PluginMain.Singleton.Config.DelayBeforeEvents, LightingMalfunction);
            }


            // Обеззараживание ТЗС
            if (PluginMain.Singleton.Config.ChanceList.TryGetValue(HczDecontaminationKey, out byte chance4)
            && CommonExtensions.ChanceChecker(chance4))
            {
                secondsLeft = PluginMain.Singleton.Config.HczDecontaminationDuration;
                _coroutines.Add(Timing.RunCoroutine(HczDecontamination()));
            }


            // Обеззараживание ЛЗС через 10 минут
            if (PluginMain.Singleton.Config.ChanceList.TryGetValue(LczDecontamination10MinKey, out byte chance5)
            && CommonExtensions.ChanceChecker(chance5))
            {
                DecontaminationController.Singleton.TimeOffset = 300f;
            }


            // Обеззараживание ЛЗС через 5 минут
            if (PluginMain.Singleton.Config.ChanceList.TryGetValue(LczDecontamination5MinKey, out byte chance6)
            && CommonExtensions.ChanceChecker(chance6))
            {
                DecontaminationController.Singleton.TimeOffset = 600f;
            }
        }

        public void OnRoundEnded()
        {
            foreach (var coroutine in _coroutines)
            {
                Timing.KillCoroutines(coroutine);
            }

            _coroutines.Clear();
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
            var cassie = PluginMain.Singleton.Config.BlockLczCheckpointsCassie;
            Exiled.API.Features.Cassie.MessageTranslated(cassie.Cassie, cassie.Translation);

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
                Exiled.API.Features.Cassie.MessageTranslated(cassie.Cassie,cassie.Translation);
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
