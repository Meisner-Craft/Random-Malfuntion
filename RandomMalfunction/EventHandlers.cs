using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using LightContainmentZoneDecontamination;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomMalfunction
{
    public class EventHandlers : CustomEventsHandler
    {
        public override void OnServerRoundStarted()
        {
            // обеззараживание LCZ произойдёт спустя 10 минут
            if (RandomPercentage(35))
                DecontaminationController.Singleton.TimeOffset = 300f;


            // обеззараживание LCZ произойдёт спустя 5 минут
            if (RandomPercentage(10))
                DecontaminationController.Singleton.TimeOffset = 600f;


            // спустя 15 минут произойдёт обеззараживание HCZ
            if (RandomPercentage(25))
                Timing.RunCoroutine(HczDecontamination());


            // Сбои света
            if (RandomPercentage(20))
                Timing.RunCoroutine(LightingMalfunction());


            // Открытие всех дверей
            if (RandomPercentage(5))
                OpenAllDoor();


            // Блокировках всех КПП в ЛЗС
            if (RandomPercentage(10))
                Timing.RunCoroutine(BlockAllLczCheckpoints());


            base.OnServerRoundStarted();
        }

        public bool RandomPercentage(int percentage)
        {
            Random rand = new Random();

            return rand.Next(100) < percentage;
        }

        private IEnumerator<float> HczDecontamination()
        {
            // Ждем 1 минуту
            yield return Timing.WaitForSeconds(60);

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
                if( door.Zone == MapGeneration.FacilityZone.HeavyContainment)
                {
                    door.IsOpened = false;
                    door.IsLocked = true;
                }
            }

            // Закрытие чекпоинтов в ТЗС
            LabApi.Features.Wrappers.Map.Doors.FirstOrDefault((Door d) => d.DoorName == DoorName.HczCheckpoint).IsOpened = false;

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
            yield return Timing.WaitForSeconds(60f);

            // Кейси
            Exiled.API.Features.Cassie.MessageTranslated(
                PluginMain.Singleton.Config.LightingMalfunctionCassie,
                PluginMain.Singleton.Config.LightingMalfunctionTranslation);

            // Получаем данные из конфига
            float duration = PluginMain.Singleton.Config.LightingMalfunctionDuration;
            float periodicity = PluginMain.Singleton.Config.LightingMalfunctionPeriodicity;

            float score = 0f;
            bool lightState = true;

            while (score < duration)
            {
                if(score % periodicity == 0f) // Каждые periodicity секунд
                {
                    if(lightState)
                    {   // Выключение света если включен
                        LabApi.Features.Wrappers.Map.TurnOffLights();
                        lightState = true;
                    }
                    else
                    {   // Включение свет если выключен
                        LabApi.Features.Wrappers.Map.TurnOnLights();
                        lightState = false;
                    }
                }

                Timing.WaitForSeconds(1f);
                score ++; // Увеличиваем счетчик
            }
            // Включение света в конце
            LabApi.Features.Wrappers.Map.TurnOnLights();
        }

        private IEnumerator<float> OpenAllDoor()
        {
            // Ждем 1 минуту
            yield return Timing.WaitForSeconds(60f);

            // Кейси
            Exiled.API.Features.Cassie.MessageTranslated(
                PluginMain.Singleton.Config.DoorMalfunctionCassie,
                PluginMain.Singleton.Config.DoorMalfunctionTranslation);

            // Открытие всех дверей
            foreach (Door door in Door.List)
            {
                door.IsOpened = true;
            }
        }

        private IEnumerator<float> BlockAllLczCheckpoints()
        {
            // Блокировка КПП в ЛЗС
            LabApi.Features.Wrappers.Map.Doors.FirstOrDefault((Door d) => d.DoorName == DoorName.LczCheckpointA).IsLocked = true;
            LabApi.Features.Wrappers.Map.Doors.FirstOrDefault((Door d) => d.DoorName == DoorName.LczCheckpointB).IsLocked = true;

            // Кейси
            Exiled.API.Features.Cassie.MessageTranslated(
                PluginMain.Singleton.Config.BlockAllLczCheckpointsCassie,
                PluginMain.Singleton.Config.BlockAllLczCheckpointsTranslation);

            // Ждем 2 минуты
            yield return Timing.WaitForSeconds(120f);

            // Разблокировка всех КПП в ЛЗС
            LabApi.Features.Wrappers.Map.Doors.FirstOrDefault((Door d) => d.DoorName == DoorName.LczCheckpointA).IsLocked = false;
            LabApi.Features.Wrappers.Map.Doors.FirstOrDefault((Door d) => d.DoorName == DoorName.LczCheckpointB).IsLocked = false;
        }
    }
}
