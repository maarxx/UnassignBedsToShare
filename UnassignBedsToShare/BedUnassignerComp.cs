using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace UnassignBedsToShare
{
    public class BedUnassignerComp : ThingComp
    {
        private Building_Bed Bed => (Building_Bed)this.parent;
        private bool shouldKeepBed;

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref shouldKeepBed, "shouldKeepBed", false);
        }

        public override void CompTickRare()
        {
            Building_Bed bed = Bed;
            List<Pawn> assignedPawns = bed.CompAssignableToPawn.AssignedPawnsForReading;
            if (!shouldKeepBed && assignedPawns.Any() && bed.Faction == Faction.OfPlayer)
            {
                assignedPawns = assignedPawns.ListFullCopy();
                foreach (Pawn assignedPawn in assignedPawns)
                {
                    if (assignedPawn.CurJob.targetA != bed)
                    {
                        bed.CompAssignableToPawn.TryUnassignPawn(assignedPawn);
                    }
                }
            }
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref shouldKeepBed, "shouldKeepBed");

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }
            if (Bed.CompAssignableToPawn.PlayerCanSeeAssignments)
            {
                Command_Toggle command_Toggle2 = new Command_Toggle();
                command_Toggle2.defaultLabel = "Keep Bed Assignment";
                command_Toggle2.defaultDesc = "Always keep the bed assignment for this bed. Otherwise, automatically unassign any pawns if they are awake.";
                command_Toggle2.hotKey = null;
                command_Toggle2.icon = DefDatabase<ThingDef>.GetNamed("Apparel_Crown").uiIcon;
                command_Toggle2.isActive = (() => shouldKeepBed);
                command_Toggle2.toggleAction = delegate
                {
                    shouldKeepBed = !shouldKeepBed;
                };
                yield return command_Toggle2;
            }
        }
    }
}
