using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Roofminer
{
  public class Designator_Roofmine: Designator_Mine {

		private static readonly IntVec3[] NeighborCoords = new IntVec3[] {
			new IntVec3( 0,  0, -1),
			new IntVec3( 1,  0,  0),
			new IntVec3( 0,  0,  1),
			new IntVec3(-1,  0,  0),
		};

		public override int DraggableDimensions => 0;

		public Designator_Roofmine() {
			this.defaultLabel = "DesignatorRoofmine".Translate();
			this.icon = ContentFinder<Texture2D>.Get("Designators/Roofmine", true);
			this.defaultDesc = "DesignatorRoofmineDesc".Translate();
		}

		public override void DesignateSingleCell(IntVec3 loc) {
			RoofDef originalLocRoof = base.Map.roofGrid.RoofAt(loc);
			Queue<IntVec3> locQueue = new Queue<IntVec3>();
			HashSet<IntVec3> locQueued = new HashSet<IntVec3>();
			locQueue.Enqueue(loc);
			locQueued.Add(loc);
			int numDesignated = 0;
			while (locQueue.Count > 0 && numDesignated <= 1000) {
				loc = locQueue.Dequeue();
				// Log.Message("Deqeueing " + loc.ToString());
				if (!loc.InBounds(base.Map))
					continue;
				// Log.Message("getting locThing");
				Thing locThing = loc.GetFirstMineable(base.Map);
				// Log.Message("checking locThing==null");
				if (locThing == null)
					continue;
				// Log.Message("checking CanDesignateThing");
				if (!this.CanDesignateThing(locThing).Accepted)
					continue;
				// Log.Message("getting locRoof");
				RoofDef locRoof = base.Map.roofGrid.RoofAt(loc);
				// Log.Message("checking one null roof");
				if (locRoof == null && originalLocRoof != null ||
				    locRoof != null && originalLocRoof == null)
					continue;
				// Log.Message("checking roof defNames match");
				if (locRoof != null && originalLocRoof != null &&
				    locRoof.defName != originalLocRoof.defName)
					continue;
				// Log.Message("checking DesignationAt");
				if (base.Map.designationManager.DesignationAt(loc, DesignationDefOf.Mine) != null)
				  continue;
	 			// Log.Message("Designating " + loc.ToString());
				base.DesignateSingleCell(loc);
				numDesignated++;
				foreach (IntVec3 delta in NeighborCoords) {
					IntVec3 newLoc = loc + delta;
					if (!locQueued.Contains(newLoc)) {
						// Log.Message("Enqueueing " + newLoc.ToString());
						locQueue.Enqueue(newLoc);
						locQueued.Add(newLoc);
					}
				}
			}
		}

	}
}
