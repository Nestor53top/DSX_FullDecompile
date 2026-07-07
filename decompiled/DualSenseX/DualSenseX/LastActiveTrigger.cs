namespace DualSenseX;

public class LastActiveTrigger
{
	public CustomTrigger SaveFile_CustomTriggerLeft = new CustomTrigger();

	public CustomTrigger SaveFile_CustomTriggerRight = new CustomTrigger();

	public ResistanceTrigger SaveFile_ResistanceTriggerLeft = new ResistanceTrigger();

	public ResistanceTrigger SaveFile_ResistanceTriggerRight = new ResistanceTrigger();

	public BowTrigger SaveFile_BowTriggerLeft = new BowTrigger();

	public BowTrigger SaveFile_BowTriggerRight = new BowTrigger();

	public GallopingTrigger SaveFile_GallopingTriggerLeft = new GallopingTrigger();

	public GallopingTrigger SaveFile_GallopingTriggerRight = new GallopingTrigger();

	public SemiAutomaticGunTrigger SaveFile_SemiAutomaticGunTriggerLeft = new SemiAutomaticGunTrigger();

	public SemiAutomaticGunTrigger SaveFile_SemiAutomaticGunTriggerRight = new SemiAutomaticGunTrigger();

	public AutomaticGunTrigger SaveFile_AutomaticGunTriggerLeft = new AutomaticGunTrigger();

	public AutomaticGunTrigger SaveFile_AutomaticGunTriggerRight = new AutomaticGunTrigger();

	public MachineTrigger SaveFile_MachineTriggerLeft = new MachineTrigger();

	public MachineTrigger SaveFile_MachineTriggerRight = new MachineTrigger();

	public VibrateTrigger SaveFile_VibrateTriggerLeft = new VibrateTrigger();

	public VibrateTrigger SaveFile_VibrateTriggerRight = new VibrateTrigger();

	public TriggerModes SaveFile_LastTriggerStateLeftMode { get; set; }

	public TriggerModes SaveFile_LastTriggerStateRightMode { get; set; }
}
