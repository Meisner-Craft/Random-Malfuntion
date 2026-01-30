using Exiled.API.Interfaces;
using System.ComponentModel;

namespace RandomMalfunction
{
    public class Config
    {
        // Список CASSIE
        [Description("Обеззараживание ТЗС через 1 минуту")]
        public string Decontamination1MinuteCassie { get; set; } = "danger . heavy containment zone overall decontamination in t minus 1 minutes";
        public string Decontamination1MinuteTranslation { get; set; } = "Опасность, процесс обеззараживание <color=#900020>тяжёлой зоны содержания</color> начнется через 1 минуту.";

        [Description("Обеззараживание ТЗС через 5 минут")]
        public string Decontamination5MinuteCassie { get; set; } = "danger . heavy containment zone overall decontamination in t minus 5 minutes";
        public string Decontamination5MinuteTranslation { get; set; } = "Опасность, процесс обеззараживание <color=#900020>тяжёлой зоны содержания</color> начнется через 5 минут.";

        [Description("Обеззараживание ТЗС через 10 минут")]
        public string Decontamination10MinuteCassie { get; set; } = "danger . heavy containment zone overall decontamination in t minus 10 minutes";
        public string Decontamination10MinuteTranslation { get; set; } = "Опасность, процесс обеззараживание <color=#900020>тяжёлой зоны содержания</color> начнется через 10 минут.";

        [Description("Обеззараживание ТЗС через 15 минут")]
        public string Decontamination15MinuteCassie { get; set; } = "attention all personnel . the heavy containment zone decontamination process will begun in t minus 15 minutes . all biological substantial must be moved to avoid destruction";
        public string Decontamination15MinuteTranslation { get; set; } = "Внимание всему персоналу! <split>Обеззараживание <color=#900020>тяжёлой зоны содержания</color> начнется через 15 минут. <split> Все биологические субстанции должны быть удалены во избежание уничтожения.";


        [Description("Блокировка чекпоинтов в ЛЗС")]
        public string BlockAllLczCheckpointsCassie { get; set; } = "$pitch_0.97 attention . initiated emergency lockdown protocol . the light containment zone is closed in 2 minutes";
        public string BlockAllLczCheckpointsTranslation { get; set; } = "Внимание! Инициирован чрезвычайный изоляционный протокол. <color=#e5df9e>Легкая зона содержания</color> заблокирована на 2 минуты!";


        [Description("Поломка света")]
        public string LightingMalfunctionCassie { get; set; } = "$PITCH_0.21 .G4 $PITCH_0.18 .G4 $PITCH_0.95 . attention . emergency power outage for $PITCH_0.1 .G2 $PITCH_0.93 . minutes . . $PITCH_0.09 .g5 .g2";
        public string LightingMalfunctionTranslation { get; set; } = "<split> Внимание! <split> Аварийное отключение света во всём учреждении на ░ минут...";


        [Description("Поломка дверей")]
        public string DoorMalfunctionCassie { get; set; } = "$pitch_0.96 attention . overall door control system malfunction detected";
        public string DoorMalfunctionTranslation { get; set; } = "Внимание! <split> Обнаружены многочисленные неисправности в системе дистанционного управления дверьми учреждения!";


        // Настройки поломки света
        [Description("Сколько секунд будет мерцать свет при поломке")]
        public float LightingMalfunctionDuration { get; set; } = 5f;

        [Description("Сколько секунд будет между мерцанием света")]
        public float LightingMalfunctionPeriodicity { get; set; } = 30f; 
    }
}