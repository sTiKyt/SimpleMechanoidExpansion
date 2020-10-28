using RimWorld;
using Verse;


namespace SME_LightEmitter
{
    public static class SME_LightEmitter
    {
        public static ThingDef SME_LightEmitterDef
        {
            get
            {
                return ThingDef.Named("SME_LightEmitter");
            }
        }
    }

    public class Light : Pawn
    {
        public const int updatePeriodInTicks = GenTicks.TicksPerRealSecond;
        public int nextUpdateTick = 0;
        public bool needSynch = true;

        public Thing light;
        public bool lightState = false;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look<Thing>(ref this.light, "light");
            Scribe_Values.Look<bool>(ref this.lightState, "lightState");
            Scribe_Values.Look<int>(ref this.nextUpdateTick, "nextUpdateTick");
            Scribe_Values.Look<bool>(ref this.needSynch, "needSynch");
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.needSynch = true;
        }

        public override void Tick()
        {
            base.Tick();
            if (this.lightState || (Find.TickManager.TicksGame >= this.nextUpdateTick))
            {
                this.nextUpdateTick = Find.TickManager.TicksGame + updatePeriodInTicks;

                if (this.needSynch && (this.Target != null))
                {
                    //          SynchLightState();
                    this.needSynch = false;
                }
                RefrestLightState();
            }
        }

        //public void SynchLightState()
        //{
        //}

        public Pawn Target;

        public bool ComputeLightState()
        {
            if ((this.Target == null) || this.Target.Dead) { return false; }
            else { return true; }
        }

        public void RefrestLightState()
        {
            bool lightShouldBeOn = ComputeLightState();
            if (lightShouldBeOn) { SwitchLightOn(); } else { SwitchLightOff(); }
        }

        public void SwitchLightOff()
        {
            if (this.light.DestroyedOrNull() == false)
            {
                this.light.Destroy();
                this.light = null;
            }
            this.lightState = false;
        }
        public void SwitchLightOn()
        {
            IntVec3 newPosition = this.Target.DrawPos.ToIntVec3();

            if ((this.light.DestroyedOrNull() == false) && (newPosition != this.light.Position))
            {
                SwitchLightOff();
            }

            if (this.light.DestroyedOrNull())
            {
                Thing potentialLight = newPosition.GetFirstThing(this.Target.Map, SME_LightEmitter.SME_LightEmitterDef);
                if (potentialLight == null)
                {
                    this.light = GenSpawn.Spawn(SME_LightEmitter.SME_LightEmitterDef, newPosition, this.Target.Map);
                }
            }
            this.lightState = true;
        }

    }
}