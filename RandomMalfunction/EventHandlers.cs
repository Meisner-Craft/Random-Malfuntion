using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.Handlers;
using LightContainmentZoneDecontamination;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomMalfunction
{
    public class EventHandlers
    {
        public static object Singleton { get; set; }

        public void RoundStarted()
        {
            // обеззараживание LCZ произойдёт спустя 10 минут
            if (RandomPercentage(PluginMain.Singleton.Config.DecontaminationLcz10MinuteChange))
                DecontaminationController.Singleton.TimeOffset = 300f;


            // обеззараживание LCZ произойдёт спустя 5 минут
            if (RandomPercentage(PluginMain.Singleton.Config.DecontaminationLcz5MinuteChange))
                DecontaminationController.Singleton.TimeOffset = 600f;


            // спустя 15 минут произойдёт обеззараживание HCZ
            if (RandomPercentage(PluginMain.Singleton.Config.DecontaminationHczChance))
                Timing.RunCoroutine(HczDecontamination());


            // Сбои света
            if (RandomPercentage(PluginMain.Singleton.Config.LightingMalfunctionChance))
                Timing.RunCoroutine(LightingMalfunction());


            // Открытие всех дверей
            if (RandomPercentage(PluginMain.Singleton.Config.DoorMalfunctionChance))
                Timing.RunCoroutine(OpenAllDoor());


            // Блокировках всех КПП в ЛЗС
            if (RandomPercentage(PluginMain.Singleton.Config.BlockAllLczCheckpointsChance))
                Timing.RunCoroutine(BlockAllLczCheckpoints());
        }

        public bool RandomPercentage(int percentage)
        {
            Random rand = new Random();

            return rand.Next(100) < percentage;
        }

        private IEnumerator<float> HczDecontamination()
        {
            yield return Timing.WaitForSeconds(PluginMain.Singleton.Config.BeforeDuration);

            // Кейси
            Exiled.API.Features.Cassie.MessageTranslated(
                PluginMain.Singleton.Config.Decontamination15MinuteCassie,
                PluginMain.Singleton.Config.Decontamination15MinuteTranslation);

            // Ждем 5 минут
            yield return Timing.WaitForSeconds(300);

            // Кейси
            Exiled.API.Features.Cassie.MessageTranslated(
                PluginMain.Singleton.Config.Decontamination10MinuteCassie,
                PluginMain.Singleton.Config.Decontamination10MinuteTranslation);

            // Ждем 5 минут
            yield return Timing.WaitForSeconds(300);

            // Кейси
            Exiled.API.Features.Cassie.MessageTranslated(
                PluginMain.Singleton.Config.Decontamination5MinuteCassie,
                PluginMain.Singleton.Config.Decontamination5MinuteTranslation);

            // Ждем 4 минуты
            yield return Timing.WaitForSeconds(240);

            // Кейси про 1 минуту
            Exiled.API.Features.Cassie.MessageTranslated(
                PluginMain.Singleton.Config.Decontamination1MinuteCassie,
                PluginMain.Singleton.Config.Decontamination1MinuteTranslation);

            // Ждем 1 минуту
            yield return Timing.WaitForSeconds(60);


            // Открытие и блок всех дверей в ТЗС
            foreach (Door door in Door.List)
            {
                if (door.Zone == ZoneType.HeavyContainment)
                {
                    door.IsOpen = false;
                    door.Lock(float.PositiveInfinity, DoorLockType.DecontLockdown);
                }
            }

            // Закрытие чекпоинтов в ТЗС
            Door.List.FirstOrDefault((Door d) => d.Name == DoorType.CheckpointEzHczA.ToString()).IsOpen = false;

            // Выдача игрокам эффекта обеззараживания
            foreach (var player in Exiled.API.Features.Player.List)
            {
                if (player.Zone == ZoneType.HeavyContainment)
                {
                    player.EnableEffect(EffectType.Decontaminating);
                }
            }
        }

        private IEnumerator<float> LightingMalfunction()
        {
            yield return Timing.WaitForSeconds(PluginMain.Singleton.Config.BeforeDuration);

            // Кейси
            Exiled.API.Features.Cassie.MessageTranslated(
                PluginMain.Singleton.Config.LightingMalfunctionCassie,
                PluginMain.Singleton.Config.LightingMalfunctionTranslation);

            // Получаем данные из конфига
            float duration = PluginMain.Singleton.Config.LightingMalfunctionDuration;
            float periodicity = PluginMain.Singleton.Config.LightingMalfunctionPeriodicity;

            float score = 0f;

            while (score < duration)
            {
                if(score % periodicity == 0f) // Каждые periodicity секунд
                {
                    Exiled.API.Features.Map.TurnOffAllLights(periodicity/2);
                }

                Timing.WaitForSeconds(1f);
                score ++; // Увеличиваем счетчик
            }
        }

        private IEnumerator<float> OpenAllDoor()
        {
            yield return Timing.WaitForSeconds(PluginMain.Singleton.Config.BeforeDuration);

            // Кейси
            Exiled.API.Features.Cassie.MessageTranslated(
                PluginMain.Singleton.Config.DoorMalfunctionCassie,
                PluginMain.Singleton.Config.DoorMalfunctionTranslation);

            // Открытие всех дверей
            foreach (Door door in Door.List)
            {
                door.IsOpen = true;
            }
        }

        private IEnumerator<float> BlockAllLczCheckpoints()
        {
            yield return Timing.WaitForSeconds(PluginMain.Singleton.Config.BeforeDuration);

            // Блокировка КПП в ЛЗС
            Door.List.FirstOrDefault((Door d) => d.Name == DoorType.CheckpointLczA.ToString()).Lock(
                PluginMain.Singleton.Config.BlockAllLczCheckpointsDuration, DoorLockType.Isolation);

            Door.List.FirstOrDefault((Door d) => d.Name == DoorType.CheckpointLczB.ToString()).Lock(
                PluginMain.Singleton.Config.BlockAllLczCheckpointsDuration, DoorLockType.Isolation);

            // Кейси
            Exiled.API.Features.Cassie.MessageTranslated(
                PluginMain.Singleton.Config.BlockAllLczCheckpointsCassie,
                PluginMain.Singleton.Config.BlockAllLczCheckpointsTranslation);
        }    
    }
}
