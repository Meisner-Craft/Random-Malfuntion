using Exiled.API.Enums;
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

        // Массив корутин
        private List<CoroutineHandle> _coroutines = new List<CoroutineHandle>();

        public void OnRoundStarted()
        {
            // Блок всех КПП в ЛКЗ
            if (PluginMain.Singleton.Config.ChanceList.TryGetValue(
                EventKey.BlockAllLczCheckpoint, out byte blockAllLczCheckpointChance) && CommonExtensions.ChanceChecker(blockAllLczCheckpointChance))
            {
                var cassie = PluginMain.Singleton.Translation.BlockLczCheckpointsCassie;
                Exiled.API.Features.Cassie.MessageTranslated(cassie.Message, cassie.Subtitles);

                Timing.CallDelayed(PluginMain.Singleton.Config.DelayBeforeEvents, BlockLcsChekpoints);
            }

            // Открытие всех дверей
            if (PluginMain.Singleton.Config.ChanceList.TryGetValue(
                EventKey.OpenAllDoors, out byte openAllDoorsChance) && CommonExtensions.ChanceChecker(openAllDoorsChance))
            {
                var cassie = PluginMain.Singleton.Translation.DoorMalfunctionCassie;
                Exiled.API.Features.Cassie.MessageTranslated(cassie.Message, cassie.Subtitles);

                Timing.CallDelayed(PluginMain.Singleton.Config.DelayBeforeEvents, OpenAllDoors);
            }


            // Поломка света
            if (PluginMain.Singleton.Config.ChanceList.TryGetValue(
                EventKey.LightMalfunction, out byte lightMalfunctionChance) && CommonExtensions.ChanceChecker(lightMalfunctionChance))
            {
                var cassie = PluginMain.Singleton.Translation.LightMalfunctionCassie;
                Exiled.API.Features.Cassie.MessageTranslated(cassie.Message, cassie.Subtitles);

                Timing.CallDelayed(PluginMain.Singleton.Config.DelayBeforeEvents, LightingMalfunction);
            }


            // Обеззараживание ТЗС
            if (PluginMain.Singleton.Config.ChanceList.TryGetValue(
                EventKey.HczDecontamination, out var hzDecontaminationChance) && CommonExtensions.ChanceChecker(hzDecontaminationChance))
            {
                secondsLeft = PluginMain.Singleton.Config.HczDecontaminationDuration;
                _coroutines.Add(Timing.RunCoroutine(HczDecontamination()));
            }


            // Обеззараживание ЛЗС через 10 минут
            if (PluginMain.Singleton.Config.ChanceList.TryGetValue(
                EventKey.LczDecontamination10Min, out var lczDecontamination10MinChance) && CommonExtensions.ChanceChecker(lczDecontamination10MinChance))
            {
                DecontaminationController.Singleton.TimeOffset = PluginMain.Singleton.Config.LczDecontamination10Min;
            }


            // Обеззараживание ЛЗС через 5 минут
            if (PluginMain.Singleton.Config.ChanceList.TryGetValue(
                EventKey.LczDecontamination5Min, out var lczDecontamination5MinChance) && CommonExtensions.ChanceChecker(lczDecontamination5MinChance))
            {
                DecontaminationController.Singleton.TimeOffset = PluginMain.Singleton.Config.LczDecontamination5Min;
            }
        }

        public void OnRoundEnded()
        {
            Timing.KillCoroutines(_coroutines.ToArray()); // Не работает ??

            _coroutines.Clear();
        }


        private void OpenAllDoors()
        {
            // Все двери кроме тех, что в черном списке
            foreach (var door in Exiled.API.Features.Doors.Door.List.Where(
                door => !PluginMain.Singleton.Config.OpenDoorBlackList.Contains(door.Type)))
            {
                door.IsOpen = true;
            }
        }

        private void BlockLcsChekpoints()
        {
            var cassie = PluginMain.Singleton.Translation.BlockLczCheckpointsCassie;
            Exiled.API.Features.Cassie.MessageTranslated(cassie.Message, cassie.Subtitles);

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
                Exiled.API.Features.Cassie.MessageTranslated(cassie.Message, cassie.Subtitles);
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
