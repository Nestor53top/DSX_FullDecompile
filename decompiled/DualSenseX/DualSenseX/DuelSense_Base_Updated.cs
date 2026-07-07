using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using HidSharp;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using WindowsInput;

namespace DualSenseX;

public abstract class DuelSense_Base_Updated
{
	public enum TouchPadSwipeDirection
	{
		up,
		down,
		left,
		right
	}

	private enum PowerState : byte
	{
		Discharging = 0,
		Charging = 1,
		Complete = 2,
		AbnormalVoltage = 10,
		AbnormalTemperture = 11,
		ChargingError = 15
	}

	public struct TouchpadData
	{
		public short Touchpad_0_X;

		public short Touchpad_0_Y;

		public int Touchpad_0_ID;

		public bool isTouchpad_0_Active;

		public short Touchpad_1_X;

		public short Touchpad_1_Y;

		public int Touchpad_1_ID;

		public bool isTouchpad_1_Active;

		public int NumberofFingersDetected;

		public bool LeftSideTouched;

		public bool RightSideTouched;

		public bool TopLeftCornerTouched;

		public bool TopRightCornerTouched;

		public bool BottomLeftCornerTouched;

		public bool BottomRightCornerTouched;

		public bool CenterTouched;

		public Vector2 StartPos;

		public bool TouchpadChecker;

		public int PixelDistanceToDetectSwipe;

		public bool SwipeUp;

		public bool SwipeDown;

		public bool SwipeLeft;

		public bool SwipeRight;
	}

	public static long NormalTrigger = 0L;

	public static long VerySoftTrigger = 4288708610L;

	public static long SoftTrigger = 4288693506L;

	public static long HardTrigger = 4288684034L;

	public static long VeryHardTrigger = 4288679938L;

	public static long HardestTrigger = 4294901762L;

	public static long RigidTrigger = 16711682L;

	public static long CalibrateTrigger = 4294902012L;

	public static short CustomRightTriggerValuesIndex;

	public static short CustomLeftTriggerValuesIndex;

	public static long VibrateTrigger_10Hz = 792352059457208358L;

	private static readonly uint[] ChecksumTableCRC32 = new uint[256]
	{
		3523407757u, 2768625435u, 1007455905u, 1259060791u, 3580832660u, 2724731650u, 996231864u, 1281784366u, 3705235391u, 2883475241u,
		852952723u, 1171273221u, 3686048678u, 2897449776u, 901431946u, 1119744540u, 3484811241u, 3098726271u, 565944005u, 1455205971u,
		3369614320u, 3219065702u, 651582172u, 1372678730u, 3245242331u, 3060352845u, 794826487u, 1483155041u, 3322131394u, 2969862996u,
		671994606u, 1594548856u, 3916222277u, 2657877971u, 123907689u, 1885708031u, 3993045852u, 2567322570u, 1010288u, 1997036262u,
		3887548279u, 2427484129u, 163128923u, 2126386893u, 3772416878u, 2547889144u, 248832578u, 2043925204u, 4108050209u, 2212294583u,
		450215437u, 1842515611u, 4088798008u, 2226203566u, 498629140u, 1790921346u, 4194326291u, 2366072709u, 336475711u, 1661535913u,
		4251816714u, 2322244508u, 325317158u, 1684325040u, 2766056989u, 3554254475u, 1255198513u, 1037565863u, 2746444292u, 3568589458u,
		1304234792u, 985283518u, 2852464175u, 3707901625u, 1141589763u, 856455061u, 2909332022u, 3664761504u, 1130791706u, 878818188u,
		3110715001u, 3463352047u, 1466425173u, 543223747u, 3187964512u, 3372436214u, 1342839628u, 655174618u, 3081909835u, 3233089245u,
		1505515367u, 784033777u, 2967466578u, 3352871620u, 1590793086u, 701932520u, 2679148245u, 3904355907u, 1908338681u, 112844655u,
		2564639436u, 4024072794u, 1993550816u, 30677878u, 2439710439u, 3865851505u, 2137352139u, 140662621u, 2517025534u, 3775001192u,
		2013832146u, 252678980u, 2181537457u, 4110462503u, 1812594589u, 453955339u, 2238339752u, 4067256894u, 1801730948u, 476252946u,
		2363233923u, 4225443349u, 1657960367u, 366298937u, 2343686810u, 4239843852u, 1707062198u, 314082080u, 1069182125u, 1220369467u,
		3518238081u, 2796764439u, 953657524u, 1339070498u, 3604597144u, 2715744526u, 828499103u, 1181144073u, 3748627891u, 2825434405u,
		906764422u, 1091244048u, 3624026538u, 2936369468u, 571309257u, 1426738271u, 3422756325u, 3137613171u, 627095760u, 1382516806u,
		3413039612u, 3161057642u, 752284923u, 1540473965u, 3268974039u, 3051332929u, 733688034u, 1555824756u, 3316994510u, 2998034776u,
		81022053u, 1943239923u, 3940166985u, 2648514015u, 62490748u, 1958656234u, 3988253008u, 2595281350u, 168805463u, 2097738945u,
		3825313147u, 2466682349u, 224526414u, 2053451992u, 3815530850u, 2490061300u, 425942017u, 1852075159u, 4151131437u, 2154433979u,
		504272920u, 1762240654u, 4026595636u, 2265434530u, 397988915u, 1623188645u, 4189500703u, 2393998729u, 282398762u, 1741824188u,
		4275794182u, 2312913296u, 1231433021u, 1046551979u, 2808630289u, 3496967303u, 1309403428u, 957143474u, 2684717064u, 3607279774u,
		1203610895u, 817534361u, 2847130659u, 3736401077u, 1087398166u, 936857984u, 2933784634u, 3654889644u, 1422998873u, 601230799u,
		3135200373u, 3453512931u, 1404893504u, 616286678u, 3182598252u, 3400902906u, 1510651243u, 755860989u, 3020215367u, 3271812305u,
		1567060338u, 710951396u, 3010007134u, 3295551688u, 1913130485u, 84884835u, 2617666777u, 3942734927u, 1969605100u, 40040826u,
		2607524032u, 3966539862u, 2094237127u, 198489425u, 2464015595u, 3856323709u, 2076066270u, 213479752u, 2511347954u, 3803648100u,
		1874795921u, 414723335u, 2175892669u, 4139142187u, 1758648712u, 534112542u, 2262612132u, 4057696306u, 1633981859u, 375629109u,
		2406151311u, 4167943193u, 1711886778u, 286155052u, 2282172566u, 4278190080u
	};

	private uint _VirtualXboxId;

	public HidDevice device;

	protected int WriteBufferSize;

	public int ReadBufferSize;

	public ViGEmClient client = new ViGEmClient();

	public IXbox360Controller Xbox360Controller;

	public IDualShock4Controller DualShock4Controller;

	protected byte LeftMotor;

	protected byte RightMotor;

	protected byte LedColor_R;

	protected byte LedColor_G;

	protected byte LedColor_B;

	protected MicLED MicLedStatus;

	protected PlayerLED PlayerLedStatus;

	protected PlayerLEDBrightness PlayerLedBrightness;

	protected long LeftAdaptiveTrigger;

	protected long RightAdaptiveTrigger;

	public bool checkbox1;

	public bool checkbox2;

	public bool checkbox3;

	public bool checkbox4;

	public bool checkbox5;

	public byte[] InputByteArray64 = new byte[64];

	public byte[] InputByteArray78 = new byte[78];

	public short LeftThumbX;

	public short LeftThumbY;

	public short RightThumbX;

	public short RightThumbY;

	public byte LeftTrigger;

	public byte RightTrigger;

	public bool Y;

	public bool B;

	public bool A;

	public bool X;

	public bool Up;

	public bool Right;

	public bool Down;

	public bool Left;

	public bool Mic;

	public bool PsButton;

	public bool TouchPadButton;

	public bool LeftThumb;

	public bool RightThumb;

	public bool Start;

	public bool Back;

	public bool LeftShoulder;

	public bool RightShoulder;

	public bool TouchPadSwipeUp;

	public bool TouchPadSwipeDown;

	public bool TouchPadSwipeRight;

	public bool TouchPadSwipeLeft;

	public string ConnectionType;

	public int BatteryLevel;

	public bool isCharging;

	private byte[] rawOutReportEx = new byte[63];

	private DS4_REPORT_EX outDS4Report;

	public SixAxis Motion;

	protected readonly DS4SixAxis sixAxis;

	protected const int TOUCHPAD_DATA_OFFSET = 33;

	public int RESOLUTION_X_MAX = 1920;

	public short StartTouchPadPosX;

	public short StartTouchPadPosX2;

	protected byte[] accel = new byte[6];

	protected byte[] gyro = new byte[6];

	public Vector2 firstPressPos;

	public Vector2 secondPressPos;

	public Vector2 currentSwipe;

	public bool AlreadySwiped;

	public bool PsButtonPress;

	private const int VendorId = 1356;

	private static int ProductId = 3302;

	private static bool _attached;

	private IMouseSimulator _mouseSimulator;

	private const int MovementDivider = 4000;

	private const int ScrollDivider = 10000;

	private const int RefreshRate = 60;

	public TouchpadData touchpadData;

	internal Touchpad touchpad;

	internal Mouse mouse;

	public uint VirtualXboxId => _VirtualXboxId;

	public int Gyroscope { get; set; } = 12;

	public int Accelerometer { get; set; } = 18;

	public static long VibrateTrigger(byte Freq)
	{
		return (long)(((ulong)Freq << 56) | 0xFF000000FF0026L);
	}

	protected static uint ComputeCRC32(byte[] byteData, int Size)
	{
		if (Size < 0)
		{
			throw new ArgumentOutOfRangeException("In ComputeCRC32: the Size is negative.");
		}
		uint num = 3940166985u;
		for (int i = 0; i < Size; i++)
		{
			num = ChecksumTableCRC32[(num & 0xFF) ^ byteData[i]] ^ (num >> 8);
		}
		return num;
	}

	public DuelSense_Base_Updated(HidDevice _device)
	{
		try
		{
			device = _device;
			byte[] array = new byte[device.GetMaxInputReportLength()];
			byte[] array2 = new byte[device.GetMaxOutputReportLength()];
			WriteBufferSize = array2.Length;
			ReadBufferSize = array.Length;
			_VirtualXboxId++;
			if (GlobalVar.Savefile.SaveFile_ControllerEmulation != EmulationType.OFF)
			{
				if (GlobalVar.Savefile.SaveFile_ControllerEmulation == EmulationType.Xbox360)
				{
					if (Xbox360Controller != null)
					{
						Xbox360Controller.FeedbackReceived += Controller_FeedbackReceived;
					}
					else
					{
						Xbox360Controller = client.CreateXbox360Controller();
						Xbox360Controller.Connect();
						Xbox360Controller.FeedbackReceived += Controller_FeedbackReceived;
					}
				}
				else if (DualShock4Controller != null)
				{
					DualShock4Controller.FeedbackReceived += Controller_FeedbackReceivedDS4;
					DualShock4Controller.AutoSubmitReport = true;
				}
				else
				{
					DualShock4Controller = client.CreateDualShock4Controller();
					DualShock4Controller.Connect();
					DualShock4Controller.FeedbackReceived += Controller_FeedbackReceivedDS4;
				}
			}
			StartReading(device);
			StartReadingTouchPad(device);
			touchpad = new Touchpad(0);
			mouse = new Mouse(0);
			touchpad.TouchButtonDown += mouse.touchButtonDown;
			touchpad.TouchButtonUp += mouse.touchButtonUp;
			touchpad.TouchesBegan += mouse.touchesBegan;
			touchpad.TouchesMoved += mouse.touchesMoved;
			touchpad.TouchesEnded += mouse.touchesEnded;
		}
		catch (Exception)
		{
		}
	}

	public void StartReading(HidDevice Dualsense)
	{
		new Thread((ThreadStart)delegate
		{
			Thread.CurrentThread.Name = "Read Device Data2";
			Thread.CurrentThread.IsBackground = true;
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			try
			{
				while (GlobalVar.DualSense != null && GlobalVar.IsControllerConnectedStatus)
				{
					if (GlobalVar.DualSense != null)
					{
						byte[] array = new byte[GlobalVar.DualSense.GetMaxInputReportLength()];
						byte[] array2 = GlobalVar.DualSense.Open().Read();
						if (array.Length == 78)
						{
							if (GlobalVar.Savefile.SaveFile_ControllerEmulation == EmulationType.OFF)
							{
								UpdateButtonStatus(array2, "OFF", array2[2], array2[3], array2[4], array2[5], array2[6], array2[7], array2[9], array2[10], array2[11], array2[54], array2[55]);
							}
							else if (GlobalVar.Savefile.SaveFile_ControllerEmulation == EmulationType.Xbox360)
							{
								UpdateButtonStatus(array2, "360", array2[2], array2[3], array2[4], array2[5], array2[6], array2[7], array2[9], array2[10], array2[11], array2[54], array2[55]);
							}
							else
							{
								UpdateButtonStatus(array2, "DS4", array2[2], array2[3], array2[4], array2[5], array2[6], array2[7], array2[9], array2[10], array2[11], array2[54], array2[55]);
							}
						}
						else if (GlobalVar.Savefile.SaveFile_ControllerEmulation == EmulationType.OFF)
						{
							UpdateButtonStatus(array2, "OFF", array2[1], array2[2], array2[3], array2[4], array2[5], array2[6], array2[8], array2[9], array2[10], array2[53], array2[54]);
						}
						else if (GlobalVar.Savefile.SaveFile_ControllerEmulation == EmulationType.Xbox360)
						{
							UpdateButtonStatus(array2, "360", array2[1], array2[2], array2[3], array2[4], array2[5], array2[6], array2[8], array2[9], array2[10], array2[53], array2[54]);
						}
						else
						{
							UpdateButtonStatus(array2, "DS4", array2[1], array2[2], array2[3], array2[4], array2[5], array2[6], array2[8], array2[9], array2[10], array2[53], array2[54]);
						}
						array2 = null;
						GlobalVar.DualSense.Open().Close();
					}
					Thread.Sleep(5);
				}
			}
			catch (Exception)
			{
				if (GlobalVar.Savefile.SaveFile_ControllerEmulation != EmulationType.OFF)
				{
					if (GlobalVar.Savefile.SaveFile_ControllerEmulation == EmulationType.Xbox360)
					{
						Xbox360Controller.Disconnect();
						Xbox360Controller.ResetReport();
						client.Dispose();
					}
					else
					{
						DualShock4Controller.Disconnect();
						DualShock4Controller.ResetReport();
						client.Dispose();
					}
				}
			}
		}).Start();
	}

	public void StartReadingTouchPad(HidDevice Dualsense)
	{
		new Thread((ThreadStart)delegate
		{
			Thread.CurrentThread.Name = "StartReadingTouchPad";
			Thread.CurrentThread.IsBackground = true;
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			try
			{
				while (GlobalVar.DualSense != null && GlobalVar.IsControllerConnectedStatus)
				{
					if (GlobalVar.DualSense != null)
					{
						_ = new byte[GlobalVar.DualSense.GetMaxInputReportLength()];
						byte[] deviceInputByteArray = GlobalVar.DualSense.Open().Read();
						UpdateButtonStatus2(deviceInputByteArray);
						deviceInputByteArray = null;
						GlobalVar.DualSense.Open().Close();
					}
					Thread.Sleep(200);
				}
			}
			catch (Exception)
			{
			}
		}).Start();
	}

	private void Controller_FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
	{
		if ((e.SmallMotor <= byte.MaxValue && e.SmallMotor >= 0) || (e.LargeMotor <= byte.MaxValue && e.LargeMotor >= 0))
		{
			SetVibration(e.SmallMotor, e.LargeMotor);
		}
	}

	private void Controller_FeedbackReceivedDS4(object sender, DualShock4FeedbackReceivedEventArgs e)
	{
		SetVibration(e.SmallMotor, e.LargeMotor);
	}

	public void SendData(byte[] SendArray)
	{
		if (device != null)
		{
			try
			{
				device.Open().Write(SendArray);
			}
			catch (Exception)
			{
			}
		}
	}

	public float RemapFloat(float v, float from1, float to1, float from2, float to2)
	{
		return (v - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	public byte remap(byte v, byte from1, byte to1, byte from2, byte to2)
	{
		if (v < from1)
		{
			return 0;
		}
		if (v > to2)
		{
			return byte.MaxValue;
		}
		return (byte)RemapFloat((int)v, (int)from1, (int)to1, (int)from2, (int)to2);
	}

	public unsafe void UpdateButtonStatus(byte[] DeviceInputByteArray, string Emulation, byte LX, byte LY, byte RX, byte RY, byte L2, byte R2, byte Button_DPad_State, byte Button2_Status, byte Extra_Buttons_Status, byte Battery_Status1, byte Battery_Status2)
	{
		int num = ((DeviceInputByteArray.Length == 78) ? 1 : 0);
		byte b = remap(L2, Convert.ToByte(GlobalVar.Savefile.SaveFile_LeftTriggerThreshold), byte.MaxValue, 0, byte.MaxValue);
		byte b2 = remap(R2, Convert.ToByte(GlobalVar.Savefile.SaveFile_RightTriggerThreshold), byte.MaxValue, 0, byte.MaxValue);
		if (Emulation == "OFF")
		{
			if (DeviceInputByteArray.Length == 78)
			{
				BatteryLevel = (DeviceInputByteArray[54] & 0xF) * 10;
				if ((byte)((DeviceInputByteArray[54] & 0xF0) >> 4) == 1)
				{
					isCharging = true;
				}
			}
			else
			{
				isCharging = true;
				BatteryLevel = (DeviceInputByteArray[53] & 0xF) * 10;
				_ = (byte)((DeviceInputByteArray[53] & 0xF0) >> 4);
			}
			TouchPadButton = Extra_Buttons_Status == 2;
			PsButton = Extra_Buttons_Status == 1;
			Mic = Extra_Buttons_Status == 4;
			LeftThumbX = (short)((LX << 8) - 32768);
			LeftThumbY = (short)(~((LY << 8) - 32768));
			RightThumbX = (short)((RX << 8) - 32768);
			RightThumbY = (short)(~((RY << 8) - 32768));
			LeftTrigger = L2;
			RightTrigger = R2;
			Y = (Button_DPad_State & 0x80) > 0;
			B = (Button_DPad_State & 0x40) > 0;
			A = (Button_DPad_State & 0x20) > 0;
			X = (Button_DPad_State & 0x10) > 0;
			int num2 = Button_DPad_State & 0xF;
			Up = num2 == 0 || num2 == 1 || num2 == 7;
			Right = num2 == 1 || num2 == 2 || num2 == 3;
			Down = num2 == 3 || num2 == 4 || num2 == 5;
			Left = num2 == 5 || num2 == 6 || num2 == 7;
			RightThumb = (Button2_Status & 0x80) > 0;
			LeftThumb = (Button2_Status & 0x40) > 0;
			Start = (Button2_Status & 0x20) > 0;
			Back = (Button2_Status & 0x10) > 0;
			RightShoulder = (Button2_Status & 2) > 0;
			LeftShoulder = (Button2_Status & 1) > 0;
			if (LeftShoulder)
			{
				GlobalVar.ButtonPresses.L1 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.L1 = false;
			}
			if (L2 > 0)
			{
				GlobalVar.ButtonPresses.L2 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.L2 = false;
			}
			if (LeftThumb)
			{
				GlobalVar.ButtonPresses.L3 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.L3 = false;
			}
			if (RightShoulder)
			{
				GlobalVar.ButtonPresses.R1 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.R1 = false;
			}
			if (R2 > 0)
			{
				GlobalVar.ButtonPresses.R2 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.R2 = false;
			}
			if (RightThumb)
			{
				GlobalVar.ButtonPresses.R3 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.R3 = false;
			}
			if (Up)
			{
				GlobalVar.ButtonPresses.Dpad_UP = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Dpad_UP = false;
			}
			if (Down)
			{
				GlobalVar.ButtonPresses.Dpad_Down = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Dpad_Down = false;
			}
			if (Left)
			{
				GlobalVar.ButtonPresses.Dpad_Left = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Dpad_Left = false;
			}
			if (Right)
			{
				GlobalVar.ButtonPresses.Dpad_Right = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Dpad_Right = false;
			}
			if (Y)
			{
				GlobalVar.ButtonPresses.Triangle = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Triangle = false;
			}
			if (X)
			{
				GlobalVar.ButtonPresses.Square = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Square = false;
			}
			if (B)
			{
				GlobalVar.ButtonPresses.Circle = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Circle = false;
			}
			if (A)
			{
				GlobalVar.ButtonPresses.Cross = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Cross = false;
			}
			if (PsButton)
			{
				GlobalVar.ButtonPresses.PS_Button = true;
			}
			else
			{
				GlobalVar.ButtonPresses.PS_Button = false;
			}
			if (TouchPadButton)
			{
				GlobalVar.ButtonPresses.Touchpad = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Touchpad = false;
			}
			if (Mic)
			{
				GlobalVar.ButtonPresses.Mic_Button = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Mic_Button = false;
			}
			if (GlobalVar.Savefile.SaveFile_EnableTouchpadToMouse)
			{
				touchpad.handleTouchpad(DeviceInputByteArray, Extra_Buttons_Status == 2);
			}
		}
		else if (Emulation == "360")
		{
			if (Xbox360Controller == null)
			{
				return;
			}
			if (DeviceInputByteArray.Length == 78)
			{
				BatteryLevel = (DeviceInputByteArray[54] & 0xF) * 10;
				if ((byte)((DeviceInputByteArray[54] & 0xF0) >> 4) == 1)
				{
					isCharging = true;
				}
			}
			else
			{
				isCharging = true;
				BatteryLevel = (DeviceInputByteArray[53] & 0xF) * 10;
				_ = (byte)((DeviceInputByteArray[53] & 0xF0) >> 4);
			}
			TouchPadButton = Extra_Buttons_Status == 2;
			PsButton = Extra_Buttons_Status == 1;
			Mic = Extra_Buttons_Status == 4;
			Xbox360Controller.SetAxisValue(Xbox360Axis.LeftThumbX, LeftThumbX = (short)((LX << 8) - 32768));
			Xbox360Controller.SetAxisValue(Xbox360Axis.LeftThumbY, LeftThumbY = (short)(~((LY << 8) - 32768)));
			Xbox360Controller.SetAxisValue(Xbox360Axis.RightThumbX, RightThumbX = (short)((RX << 8) - 32768));
			Xbox360Controller.SetAxisValue(Xbox360Axis.RightThumbY, RightThumbY = (short)(~((RY << 8) - 32768)));
			Xbox360Controller.SetSliderValue(Xbox360Slider.LeftTrigger, LeftTrigger = b);
			Xbox360Controller.SetSliderValue(Xbox360Slider.RightTrigger, RightTrigger = b2);
			Xbox360Controller.SetButtonState(Xbox360Button.Y, Y = (Button_DPad_State & 0x80) > 0);
			Xbox360Controller.SetButtonState(Xbox360Button.B, B = (Button_DPad_State & 0x40) > 0);
			Xbox360Controller.SetButtonState(Xbox360Button.A, A = (Button_DPad_State & 0x20) > 0);
			Xbox360Controller.SetButtonState(Xbox360Button.X, X = (Button_DPad_State & 0x10) > 0);
			int num3 = Button_DPad_State & 0xF;
			Xbox360Controller.SetButtonState(Xbox360Button.Up, Up = num3 == 0 || num3 == 1 || num3 == 7);
			Xbox360Controller.SetButtonState(Xbox360Button.Right, Right = num3 == 1 || num3 == 2 || num3 == 3);
			Xbox360Controller.SetButtonState(Xbox360Button.Down, Down = num3 == 3 || num3 == 4 || num3 == 5);
			Xbox360Controller.SetButtonState(Xbox360Button.Left, Left = num3 == 5 || num3 == 6 || num3 == 7);
			Xbox360Controller.SetButtonState(Xbox360Button.RightThumb, RightThumb = (Button2_Status & 0x80) > 0);
			Xbox360Controller.SetButtonState(Xbox360Button.LeftThumb, LeftThumb = (Button2_Status & 0x40) > 0);
			Xbox360Controller.SetButtonState(Xbox360Button.Start, Start = (Button2_Status & 0x20) > 0);
			Xbox360Controller.SetButtonState(Xbox360Button.Back, Back = (Button2_Status & 0x10) > 0);
			Xbox360Controller.SetButtonState(Xbox360Button.RightShoulder, RightShoulder = (Button2_Status & 2) > 0);
			Xbox360Controller.SetButtonState(Xbox360Button.LeftShoulder, LeftShoulder = (Button2_Status & 1) > 0);
			Xbox360Controller.SubmitReport();
			if (LeftShoulder)
			{
				GlobalVar.ButtonPresses.L1 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.L1 = false;
			}
			if (L2 > 0)
			{
				GlobalVar.ButtonPresses.L2 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.L2 = false;
			}
			if (LeftThumb)
			{
				GlobalVar.ButtonPresses.L3 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.L3 = false;
			}
			if (RightShoulder)
			{
				GlobalVar.ButtonPresses.R1 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.R1 = false;
			}
			if (R2 > 0)
			{
				GlobalVar.ButtonPresses.R2 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.R2 = false;
			}
			if (RightThumb)
			{
				GlobalVar.ButtonPresses.R3 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.R3 = false;
			}
			if (Up)
			{
				GlobalVar.ButtonPresses.Dpad_UP = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Dpad_UP = false;
			}
			if (Down)
			{
				GlobalVar.ButtonPresses.Dpad_Down = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Dpad_Down = false;
			}
			if (Left)
			{
				GlobalVar.ButtonPresses.Dpad_Left = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Dpad_Left = false;
			}
			if (Right)
			{
				GlobalVar.ButtonPresses.Dpad_Right = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Dpad_Right = false;
			}
			if (Y)
			{
				GlobalVar.ButtonPresses.Triangle = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Triangle = false;
			}
			if (X)
			{
				GlobalVar.ButtonPresses.Square = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Square = false;
			}
			if (B)
			{
				GlobalVar.ButtonPresses.Circle = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Circle = false;
			}
			if (A)
			{
				GlobalVar.ButtonPresses.Cross = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Cross = false;
			}
			if (PsButton)
			{
				GlobalVar.ButtonPresses.PS_Button = true;
			}
			else
			{
				GlobalVar.ButtonPresses.PS_Button = false;
			}
			if (TouchPadButton)
			{
				GlobalVar.ButtonPresses.Touchpad = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Touchpad = false;
			}
			if (Mic)
			{
				GlobalVar.ButtonPresses.Mic_Button = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Mic_Button = false;
			}
			if (GlobalVar.Savefile.SaveFile_EnableTouchpadToMouse)
			{
				touchpad.handleTouchpad(DeviceInputByteArray, Extra_Buttons_Status == 2);
			}
		}
		else
		{
			if (DualShock4Controller == null)
			{
				return;
			}
			if (DeviceInputByteArray.Length == 78)
			{
				BatteryLevel = (DeviceInputByteArray[54] & 0xF) * 10;
				if ((byte)((DeviceInputByteArray[54] & 0xF0) >> 4) == 1)
				{
					isCharging = true;
				}
			}
			else
			{
				isCharging = true;
				BatteryLevel = (DeviceInputByteArray[53] & 0xF) * 10;
				_ = (byte)((DeviceInputByteArray[53] & 0xF0) >> 4);
			}
			byte bIsUpTrackingNum = DeviceInputByteArray[33 + num];
			_ = DeviceInputByteArray[33 + num];
			_ = DeviceInputByteArray[33 + num];
			short num4 = (short)(((ushort)(DeviceInputByteArray[35 + num] & 0xF) << 8) | DeviceInputByteArray[34 + num]);
			short num5 = (short)((DeviceInputByteArray[36 + num] << 4) | ((ushort)(DeviceInputByteArray[35 + num] & 0xF0) >> 4));
			byte bIsUpTrackingNum2 = DeviceInputByteArray[37 + num];
			_ = DeviceInputByteArray[37 + num];
			_ = DeviceInputByteArray[37 + num];
			short num6 = (short)(((ushort)(DeviceInputByteArray[39 + num] & 0xF) << 8) | DeviceInputByteArray[38 + num]);
			short num7 = (short)((DeviceInputByteArray[40 + num] << 4) | ((ushort)(DeviceInputByteArray[39 + num] & 0xF0) >> 4));
			int num8 = 0;
			byte bPacketCounter = DeviceInputByteArray[41 + num + num8];
			_ = DeviceInputByteArray[33 + num + num8] >> 7 == 0;
			_ = DeviceInputByteArray[33 + num + num8];
			bool flag = DeviceInputByteArray[37 + num + num8] >> 7 == 0;
			_ = DeviceInputByteArray[37 + num + num8];
			_ = ((DeviceInputByteArray[35 + num + num8] & 0xF) << 8) | DeviceInputByteArray[34 + num + num8];
			_ = RESOLUTION_X_MAX * 2 / 5;
			_ = RESOLUTION_X_MAX * 2 / 5;
			_ = DeviceInputByteArray[34];
			if (GlobalVar.Savefile.SaveFile_EnableTouchpadToMouse)
			{
				touchpad.handleTouchpad(DeviceInputByteArray, Extra_Buttons_Status == 2);
			}
			byte b3 = DeviceInputByteArray[Gyroscope];
			byte b4 = DeviceInputByteArray[Gyroscope + 1];
			byte b5 = DeviceInputByteArray[Gyroscope + 2];
			byte b6 = DeviceInputByteArray[Gyroscope + 3];
			byte b7 = DeviceInputByteArray[Gyroscope + 4];
			byte num9 = DeviceInputByteArray[Gyroscope + 5];
			short wGyroX = (short)((ushort)(b4 << 8) | b3);
			short wGyroY = (short)((ushort)(b6 << 8) | b5);
			short wGyroZ = (short)((ushort)(num9 << 8) | b7);
			byte b8 = DeviceInputByteArray[Accelerometer];
			byte b9 = DeviceInputByteArray[Accelerometer + 1];
			byte b10 = DeviceInputByteArray[Accelerometer + 2];
			byte b11 = DeviceInputByteArray[Accelerometer + 3];
			byte b12 = DeviceInputByteArray[Accelerometer + 4];
			byte num10 = DeviceInputByteArray[Accelerometer + 5];
			short num11 = (short)((ushort)(b9 << 8) | b8);
			short num12 = (short)((ushort)(b11 << 8) | b10);
			short num13 = (short)((ushort)(num10 << 8) | b12);
			_ = (float)(-num11) / 8192f;
			_ = (float)(-num12) / 8192f;
			_ = (float)(-num13) / 8192f;
			TouchPadButton = Extra_Buttons_Status == 2;
			PsButton = Extra_Buttons_Status == 1;
			Mic = Extra_Buttons_Status == 4;
			Y = (Button_DPad_State & 0x80) > 0;
			B = (Button_DPad_State & 0x40) > 0;
			A = (Button_DPad_State & 0x20) > 0;
			X = (Button_DPad_State & 0x10) > 0;
			RightThumb = (Button2_Status & 0x80) > 0;
			LeftThumb = (Button2_Status & 0x40) > 0;
			Start = (Button2_Status & 0x20) > 0;
			Back = (Button2_Status & 0x10) > 0;
			RightShoulder = (Button2_Status & 2) > 0;
			LeftShoulder = (Button2_Status & 1) > 0;
			LeftTrigger = b;
			RightTrigger = b2;
			LeftThumbX = (short)((LX << 8) - 32768);
			LeftThumbY = (short)(~((LY << 8) - 32768));
			RightThumbX = (short)((RX << 8) - 32768);
			RightThumbY = (short)(~((RY << 8) - 32768));
			TouchPadButton = Extra_Buttons_Status == 2;
			PsButton = Extra_Buttons_Status == 1;
			Mic = Extra_Buttons_Status == 4;
			ushort num14 = 0;
			DualShock4DPadDirection dualShock4DPadDirection = DualShock4DPadDirection.None;
			ushort num15 = 0;
			int num16 = Button_DPad_State & 0xF;
			Up = num16 == 0 || num16 == 1 || num16 == 7;
			Right = num16 == 1 || num16 == 2 || num16 == 3;
			Down = num16 == 3 || num16 == 4 || num16 == 5;
			Left = num16 == 5 || num16 == 6 || num16 == 7;
			bool num17 = num16 == 1;
			bool flag2 = num16 == 7;
			bool flag3 = num16 == 5;
			bool flag4 = num16 == 3;
			if ((Button2_Status & 0x10) > 0)
			{
				num14 |= DualShock4Button.Share.Value;
			}
			if ((Button2_Status & 0x40) > 0)
			{
				num14 |= DualShock4Button.ThumbLeft.Value;
			}
			if ((Button2_Status & 0x80) > 0)
			{
				num14 |= DualShock4Button.ThumbRight.Value;
			}
			if ((Button2_Status & 0x20) > 0)
			{
				num14 |= DualShock4Button.Options.Value;
			}
			if (Up)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.North;
			}
			if (Down)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.South;
			}
			if (Left)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.East;
			}
			if (Right)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.West;
			}
			if (num17)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.Northeast;
			}
			else if (flag2)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.Northwest;
			}
			else if (Up)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.North;
			}
			else if (flag4)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.Southeast;
			}
			else if (Right)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.East;
			}
			else if (flag3)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.Southwest;
			}
			else if (Down)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.South;
			}
			else if (Left)
			{
				dualShock4DPadDirection = DualShock4DPadDirection.West;
			}
			if ((Button2_Status & 1) > 0)
			{
				num14 |= DualShock4Button.ShoulderLeft.Value;
			}
			if ((Button2_Status & 2) > 0)
			{
				num14 |= DualShock4Button.ShoulderRight.Value;
			}
			if (L2 > 0)
			{
				num14 |= DualShock4Button.TriggerLeft.Value;
			}
			if (R2 > 0)
			{
				num14 |= DualShock4Button.TriggerRight.Value;
			}
			if ((Button_DPad_State & 0x80) > 0)
			{
				num14 |= DualShock4Button.Triangle.Value;
			}
			if ((Button_DPad_State & 0x40) > 0)
			{
				num14 |= DualShock4Button.Circle.Value;
			}
			if ((Button_DPad_State & 0x20) > 0)
			{
				num14 |= DualShock4Button.Cross.Value;
			}
			if ((Button_DPad_State & 0x10) > 0)
			{
				num14 |= DualShock4Button.Square.Value;
			}
			if (PsButton)
			{
				num15 |= DualShock4SpecialButton.Ps.Value;
			}
			if (TouchPadButton)
			{
				num15 |= DualShock4SpecialButton.Touchpad.Value;
			}
			outDS4Report.wButtons = num14;
			outDS4Report.bSpecial = (byte)num15;
			outDS4Report.wButtons |= dualShock4DPadDirection.Value;
			outDS4Report.bThumbLX = LX;
			outDS4Report.bThumbLY = LY;
			outDS4Report.bThumbRX = RX;
			outDS4Report.bThumbRY = RY;
			outDS4Report.bTriggerL = b;
			outDS4Report.bTriggerR = b2;
			if (LeftShoulder)
			{
				GlobalVar.ButtonPresses.L1 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.L1 = false;
			}
			if (L2 > 0)
			{
				GlobalVar.ButtonPresses.L2 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.L2 = false;
			}
			if (LeftThumb)
			{
				GlobalVar.ButtonPresses.L3 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.L3 = false;
			}
			if (RightShoulder)
			{
				GlobalVar.ButtonPresses.R1 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.R1 = false;
			}
			if (R2 > 0)
			{
				GlobalVar.ButtonPresses.R2 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.R2 = false;
			}
			if (RightThumb)
			{
				GlobalVar.ButtonPresses.R3 = true;
			}
			else
			{
				GlobalVar.ButtonPresses.R3 = false;
			}
			if (Up)
			{
				GlobalVar.ButtonPresses.Dpad_UP = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Dpad_UP = false;
			}
			if (Down)
			{
				GlobalVar.ButtonPresses.Dpad_Down = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Dpad_Down = false;
			}
			if (Left)
			{
				GlobalVar.ButtonPresses.Dpad_Left = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Dpad_Left = false;
			}
			if (Right)
			{
				GlobalVar.ButtonPresses.Dpad_Right = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Dpad_Right = false;
			}
			if (Y)
			{
				GlobalVar.ButtonPresses.Triangle = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Triangle = false;
			}
			if (X)
			{
				GlobalVar.ButtonPresses.Square = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Square = false;
			}
			if (B)
			{
				GlobalVar.ButtonPresses.Circle = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Circle = false;
			}
			if (A)
			{
				GlobalVar.ButtonPresses.Cross = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Cross = false;
			}
			if (PsButton)
			{
				GlobalVar.ButtonPresses.PS_Button = true;
			}
			else
			{
				GlobalVar.ButtonPresses.PS_Button = false;
			}
			if (TouchPadButton)
			{
				GlobalVar.ButtonPresses.Touchpad = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Touchpad = false;
			}
			if (Mic)
			{
				GlobalVar.ButtonPresses.Mic_Button = true;
			}
			else
			{
				GlobalVar.ButtonPresses.Mic_Button = false;
			}
			outDS4Report.bTouchPacketsN = 1;
			outDS4Report.sCurrentTouch.bPacketCounter = bPacketCounter;
			outDS4Report.sCurrentTouch.bIsUpTrackingNum1 = bIsUpTrackingNum;
			outDS4Report.sCurrentTouch.bTouchData1[0] = (byte)(num4 & 0xFF);
			outDS4Report.sCurrentTouch.bTouchData1[1] = (byte)(((num4 >> 8) & 0xF) | ((num5 << 4) & 0xF0));
			outDS4Report.sCurrentTouch.bTouchData1[2] = (byte)(num5 >> 4);
			outDS4Report.sCurrentTouch.bIsUpTrackingNum2 = bIsUpTrackingNum2;
			outDS4Report.sCurrentTouch.bTouchData2[0] = (byte)(num6 & 0xFF);
			outDS4Report.sCurrentTouch.bTouchData2[1] = (byte)(((num6 >> 8) & 0xF) | ((num7 << 4) & 0xF0));
			outDS4Report.sCurrentTouch.bTouchData2[2] = (byte)(num7 >> 4);
			fixed (byte* ptr = &DeviceInputByteArray[16 + num])
			{
				fixed (byte* ptr2 = gyro)
				{
					fixed (byte* ptr3 = accel)
					{
						for (int i = 0; i < 6; i++)
						{
							ptr2[i] = ptr[i];
						}
						for (int j = 6; j < 12; j++)
						{
							ptr3[j - 6] = ptr[j];
						}
						int y = (short)((ushort)(gyro[3] << 8) | gyro[2]);
						int x = (short)((ushort)(gyro[1] << 8) | gyro[0]);
						int z = (short)((ushort)(gyro[5] << 8) | gyro[4]);
						int aX = (short)((ushort)(accel[1] << 8) | accel[0]);
						int aY = (short)((ushort)(accel[3] << 8) | accel[2]);
						int aZ = (short)((ushort)(accel[5] << 8) | accel[4]);
						Motion = new SixAxis(x, y, z, aX, aY, aZ, 0.0);
					}
				}
			}
			outDS4Report.wGyroX = wGyroX;
			outDS4Report.wGyroY = wGyroY;
			outDS4Report.wGyroZ = wGyroZ;
			outDS4Report.wAccelX = num11;
			outDS4Report.wAccelY = num12;
			outDS4Report.wAccelZ = num13;
			outDS4Report.bBatteryLvlSpecial = (byte)(Battery_Status1 / 11);
			DS4OutDeviceExtras.CopyBytes(ref outDS4Report, rawOutReportEx);
			DualShock4Controller.SubmitRawReport(rawOutReportEx);
		}
	}

	public void UpdateButtonStatus2(byte[] DeviceInputByteArray)
	{
		int num = ((DeviceInputByteArray.Length == 78) ? 1 : 0);
		_ = DeviceInputByteArray[33 + num];
		_ = DeviceInputByteArray[33 + num];
		_ = DeviceInputByteArray[33 + num];
		short x = (short)(((ushort)(DeviceInputByteArray[35 + num] & 0xF) << 8) | DeviceInputByteArray[34 + num]);
		short y = (short)((DeviceInputByteArray[36 + num] << 4) | ((ushort)(DeviceInputByteArray[35 + num] & 0xF0) >> 4));
		_ = DeviceInputByteArray[37 + num];
		_ = DeviceInputByteArray[37 + num];
		_ = DeviceInputByteArray[37 + num];
		_ = DeviceInputByteArray[39 + num];
		_ = DeviceInputByteArray[38 + num];
		_ = DeviceInputByteArray[40 + num];
		_ = DeviceInputByteArray[39 + num];
		int num2 = 0;
		_ = DeviceInputByteArray[41 + num + num2];
		bool num3 = DeviceInputByteArray[33 + num + num2] >> 7 == 0;
		_ = DeviceInputByteArray[33 + num + num2];
		bool flag = DeviceInputByteArray[37 + num + num2] >> 7 == 0;
		_ = DeviceInputByteArray[37 + num + num2];
		_ = ((DeviceInputByteArray[35 + num + num2] & 0xF) << 8) | DeviceInputByteArray[34 + num + num2];
		_ = RESOLUTION_X_MAX * 2 / 5;
		_ = RESOLUTION_X_MAX * 2 / 5;
		_ = DeviceInputByteArray[34];
		if (num3)
		{
			AlreadySwiped = false;
			TouchPadTouched(x, y);
		}
		else
		{
			onFling(x, y);
		}
	}

	public void TouchPadTouched(short x, short y)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!AlreadySwiped)
		{
			firstPressPos = new Vector2((float)x, (float)y);
			AlreadySwiped = true;
		}
	}

	public void onFling(short x, short y)
	{
		if (AlreadySwiped)
		{
			float x2 = firstPressPos.X;
			float y2 = firstPressPos.Y;
			float x3 = x;
			float y3 = y;
			TouchPadSwipeDirection direction = getDirection(x2, y2, x3, y3);
			OnSwipeAsync(direction);
			AlreadySwiped = false;
		}
	}

	public async Task<bool> OnSwipeAsync(TouchPadSwipeDirection direction)
	{
		switch (direction)
		{
		case TouchPadSwipeDirection.up:
			TouchPadSwipeUp = true;
			await Task.Delay(100);
			TouchPadSwipeUp = false;
			break;
		case TouchPadSwipeDirection.down:
			TouchPadSwipeDown = true;
			await Task.Delay(100);
			TouchPadSwipeDown = false;
			break;
		case TouchPadSwipeDirection.left:
			TouchPadSwipeLeft = true;
			await Task.Delay(100);
			TouchPadSwipeLeft = false;
			break;
		case TouchPadSwipeDirection.right:
			TouchPadSwipeRight = true;
			await Task.Delay(100);
			TouchPadSwipeRight = false;
			break;
		}
		return false;
	}

	public TouchPadSwipeDirection getDirection(float x1, float y1, float x2, float y2)
	{
		return FromAngle(getAngle(x1, y1, x2, y2));
	}

	public double getAngle(float x1, float y1, float x2, float y2)
	{
		return ((Math.Atan2(y1 - y2, x2 - x1) + Math.PI) * 180.0 / Math.PI + 180.0) % 360.0;
	}

	public static TouchPadSwipeDirection FromAngle(double angle)
	{
		if (inRange(angle, 45f, 135f))
		{
			return TouchPadSwipeDirection.up;
		}
		if (inRange(angle, 0f, 45f) || inRange(angle, 315f, 360f))
		{
			return TouchPadSwipeDirection.right;
		}
		if (inRange(angle, 225f, 315f))
		{
			return TouchPadSwipeDirection.down;
		}
		return TouchPadSwipeDirection.left;
	}

	private static bool inRange(double angle, float init, float end)
	{
		if (angle >= (double)init)
		{
			return angle < (double)end;
		}
		return false;
	}

	public void SetLeftAdaptiveTrigger(long Parameter)
	{
		SetAdaptiveTrigger(IsLeftTrigger: true, Parameter);
	}

	public void SetRightAdaptiveTrigger(long Parameter)
	{
		SetAdaptiveTrigger(IsLeftTrigger: false, Parameter);
	}

	public abstract void SetTouchpadLEDBrightness(double Percentage);

	public abstract void SetPlayerLedStatus(PlayerLED Status, bool checkbox1, bool checkbox2, bool checkbox3, bool checkbox4, bool checkbox5);

	public abstract void SetPlayerLedBrightness(PlayerLEDBrightness Status);

	public abstract void SetMicLedStatus(MicLED Status);

	public abstract void SetLedColor(byte R, byte G, byte B);

	public abstract void RefreshButtonStatus(byte[] inputReport);

	public abstract void SetVibration(byte Left, byte Right);

	public abstract void SetAdaptiveTrigger(bool IsLeftTrigger, long Parameter);

	public abstract void CheckConnection();
}
