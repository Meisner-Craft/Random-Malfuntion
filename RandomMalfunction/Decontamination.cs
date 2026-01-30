using LightContainmentZoneDecontamination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LightContainmentZoneDecontamination.DecontaminationController;

namespace RandomMalfunction
{
    public class Decontamination
    {
        DecontaminationController singleton = DecontaminationController.Singleton;



        DecontaminationPhase[] myPhases = new DecontaminationPhase[]
        {
    new DecontaminationPhase
    {
        TimeTrigger = 300f, // 5 мин
        AnnouncementLine = null,
        Function = DecontaminationPhase.PhaseFunction.None
    },
    new DecontaminationPhase
    {
        TimeTrigger = 600f, // 10 мин
        AnnouncementLine = null,
        Function = DecontaminationPhase.PhaseFunction.None
    },
    new DecontaminationPhase
    {
        TimeTrigger = 870f, // 14.5 мин
        AnnouncementLine = null,
        Function = DecontaminationPhase.PhaseFunction.OpenCheckpoints
    },
    new DecontaminationPhase
    {
        TimeTrigger = 900f, // 15 мин
        AnnouncementLine = null,
        Function = DecontaminationPhase.PhaseFunction.Final
    }
        };
    }
}
