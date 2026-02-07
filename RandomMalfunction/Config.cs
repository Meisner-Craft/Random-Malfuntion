using Exiled.API.Enums;
using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace RandomMalfunction
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Задержка перед всеми событиями")]
        public float DelayBeforeEvents { get; set; } = 30f;

        [Description("На сколько будут блокироваться КПП в ЛЗС")]
        public float LczCheckpointBlockDuration { get; set; } = 120f;

        [Description("Длительность поломки света")]
        public float LightingMalfunctionDuration { get; set; } = 30f;

        [Description("Периодичность поломки света")]
        public float LightingMalfunctionPeriodicity { get; set; } = 5f;

        [Description("Шансы срабатывания ивентов")]
        public Dictionary<string, byte> ChanceList = new Dictionary<string, byte>
        {
            {"BlockAllLczCheckpoint", 10},
            {"OpenAllDoors", 5},
            {"LightMalfunction", 20},
            {"HczDecontamination", 25},
            {"LczDecontamination10Min", 35},
            {"LczDecontamination5Min", 10}
        };

        [Description("Черный список дверей для открытия")]
        public List<DoorType> OpenDoorBlackList = new List<DoorType>
        {
            DoorType.GateA,
            DoorType.GateB,
            DoorType.Scp079First,
            DoorType.Scp079Second
        };

        [Description("Черный список дверей для открытия в ТЗС")]
        public List<DoorType> OpenDoorDecontaminationBlackList = new List<DoorType>
        {
            DoorType.Scp096,
            DoorType.Scp079First,
            DoorType.Scp079Second,
            DoorType.Scp049Gate,
            DoorType.Scp173NewGate
        };


        [Description("Общее время обеззараживания ТЗС")]
        public float HczDecontaminationDuration { get; set; } = 900f;

        [Description("Фазы в секундах")]
        public Dictionary<int, CassieMessage> CassieBySeconds { get; set; } = new Dictionary<int, CassieMessage>()
        {
            {
                900, new CassieMessage
                {
                    Cassie = "cassie sl attentionallpersonnel . the heavy containment zone decontamination process will begun in tminus 15 minutes . all biological substantial must be moved to avoid destruction",
                    Translation = "Внимание всему персоналу! <split>Обеззараживание <color=#900020>тяжёлой зоны содержания</color> начнется через 15 минут. <split> Все биологические субстанции должны быть удалены во избежание уничтожения. "
                }
            },
            {
                600, new CassieMessage
                {
                    Cassie = "cassie sl danger . heavy containment zone overall decontamination in tminus 15 minutes",
                    Translation = "Опасность, процесс обеззараживание <color=#900020>тяжёлой зоны содержания</color> начнется через 10 минут."
                }
            },
            {
                300, new CassieMessage
                {
                    Cassie = "cassie sl danger . heavy containment zone overall decontamination in tminus 5 minutes",
                    Translation = "Опасность, процесс обеззараживание <color=#900020>тяжёлой зоны содержания</color> начнется через 5 минут."
                }
            },
            {
                60, new CassieMessage
                {
                    Cassie = "cassie sl danger . heavy containment zone overall decontamination in tminus 1 minutes",
                    Translation = "Опасность, процесс обеззараживание <color=#900020>тяжёлой зоны содержания</color> начнется через 1 минуту."
                }
            }
        };

        [Description("Блокировка чекпоинтов в ЛЗС")]
        public CassieMessage BlockLczCheckpointsCassie = new CassieMessage
        {
            Cassie = "$pitch_0.97 attention . initiated emergency lockdown protocol . the light containment zone is closed in 2 minutes",
            Translation = "Внимание! Инициирован чрезвычайный изоляционный протокол. <color=#e5df9e>Легкая зона содержания</color> заблокирована на 2 минуты!"
        };

        [Description("Поломка света")]
        public CassieMessage LightMalfunctionCassie = new CassieMessage
        {
            Cassie = "$PITCH_0.21 .G4 $PITCH_0.18 .G4 $PITCH_0.95 . attention . emergency power outage for $PITCH_0.1 .G2 $PITCH_0.93 . minutes . . $PITCH_0.09 .g5 .g2",
            Translation = "<split> Внимание! <split> Аварийное отключение света во всём учреждении на ░ минут..."
        };

        [Description("Поломка дверей")]
        public CassieMessage DoorMalfunctionCassie = new CassieMessage
        {
            Cassie = "$pitch_0.96 attention . overall door control system malfunction detected",
            Translation = "Внимание! <split> Обнаружены многочисленные неисправности в системе дистанционного управления дверьми учреждения!"
        };
    }
}