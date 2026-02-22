using Exiled.API.Enums;
using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace RandomMalfunction
{
    public enum EventKey
    {
        BlockAllLczCheckpoint,
        OpenAllDoors,
        LightMalfunction,
        HczDecontamination,
        LczDecontamination10Min,
        LczDecontamination5Min
    }

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

        [Description("Время при обеззараживании ЛЗС через 10 минут")]
        public float LczDecontamination10Min { get; set; } = 300f;

        [Description("Время при обеззараживании ЛЗС через 5 минут")]
        public float LczDecontamination5Min { get; set; } = 600f;


        [Description("Шансы срабатывания ивентов")]
        public Dictionary<EventKey, byte> ChanceList = new Dictionary<EventKey, byte>
        {
            {EventKey.BlockAllLczCheckpoint, 10},
            {EventKey.OpenAllDoors, 5},
            {EventKey.LightMalfunction, 20},
            {EventKey.HczDecontamination, 25},
            {EventKey.LczDecontamination10Min, 35},
            {EventKey.LczDecontamination5Min, 10}
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
        public Dictionary<int, Exiled.API.Features.CassieMessage> CassieBySeconds { get; set; } = new Dictionary<int, Exiled.API.Features.CassieMessage>()
        {
            {
                900, new Exiled.API.Features.CassieMessage
                {
                    Message = "cassie sl attentionallpersonnel . the heavy containment zone decontamination process will begun in tminus 15 minutes . all biological substantial must be moved to avoid destruction",
                    Subtitles = "Внимание всему персоналу! <split>Обеззараживание <color=#900020>тяжёлой зоны содержания</color> начнется через 15 минут. <split> Все биологические субстанции должны быть удалены во избежание уничтожения. "
                }
            },
            {
                600, new Exiled.API.Features.CassieMessage
                {
                    Message = "cassie sl danger . heavy containment zone overall decontamination in tminus 15 minutes",
                    Subtitles = "Опасность, процесс обеззараживание <color=#900020>тяжёлой зоны содержания</color> начнется через 10 минут."
                }
            },
            {
                300, new Exiled.API.Features.CassieMessage
                {
                    Message = "cassie sl danger . heavy containment zone overall decontamination in tminus 5 minutes",
                    Subtitles = "Опасность, процесс обеззараживание <color=#900020>тяжёлой зоны содержания</color> начнется через 5 минут."
                }
            },
            {
                60, new Exiled.API.Features.CassieMessage
                {
                    Message = "cassie sl danger . heavy containment zone overall decontamination in tminus 1 minutes",
                    Subtitles = "Опасность, процесс обеззараживание <color=#900020>тяжёлой зоны содержания</color> начнется через 1 минуту."
                }
            }
        };
    }
}