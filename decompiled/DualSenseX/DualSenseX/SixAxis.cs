namespace DualSenseX;

public class SixAxis
{
	public const int ACC_RES_PER_G = 8192;

	public const float F_ACC_RES_PER_G = 8192f;

	public const int GYRO_RES_IN_DEG_SEC = 16;

	public const float F_GYRO_RES_IN_DEG_SEC = 16f;

	public int gyroYaw;

	public int gyroPitch;

	public int gyroRoll;

	public int accelX;

	public int accelY;

	public int accelZ;

	public int outputAccelX;

	public int outputAccelY;

	public int outputAccelZ;

	public bool outputGyroControls;

	public double accelXG;

	public double accelYG;

	public double accelZG;

	public double angVelYaw;

	public double angVelPitch;

	public double angVelRoll;

	public int gyroYawFull;

	public int gyroPitchFull;

	public int gyroRollFull;

	public int accelXFull;

	public int accelYFull;

	public int accelZFull;

	public double elapsed;

	public SixAxis previousAxis;

	private double tempDouble;

	public SixAxis(int X, int Y, int Z, int aX, int aY, int aZ, double elapsedDelta, SixAxis prevAxis = null)
	{
		populate(X, Y, Z, aX, aY, aZ, elapsedDelta, prevAxis);
	}

	public void copy(SixAxis src)
	{
		gyroYaw = src.gyroYaw;
		gyroPitch = src.gyroPitch;
		gyroRoll = src.gyroRoll;
		gyroYawFull = src.gyroYawFull;
		accelXFull = src.accelXFull;
		accelYFull = src.accelYFull;
		accelZFull = src.accelZFull;
		angVelYaw = src.angVelYaw;
		angVelPitch = src.angVelPitch;
		angVelRoll = src.angVelRoll;
		accelXG = src.accelXG;
		accelYG = src.accelYG;
		accelZG = src.accelZG;
		accelX = src.accelX;
		accelY = src.accelY;
		accelZ = src.accelZ;
		outputAccelX = accelX;
		outputAccelY = accelY;
		outputAccelZ = accelZ;
		elapsed = src.elapsed;
		previousAxis = src.previousAxis;
		outputGyroControls = src.outputGyroControls;
	}

	public void populate(int X, int Y, int Z, int aX, int aY, int aZ, double elapsedDelta, SixAxis prevAxis = null)
	{
		gyroYaw = -X / 256;
		gyroPitch = Y / 256;
		gyroRoll = -Z / 256;
		gyroYawFull = -X;
		gyroPitchFull = Y;
		gyroRollFull = -Z;
		accelXFull = -aX;
		accelYFull = -aY;
		accelZFull = aZ;
		angVelYaw = (float)gyroYawFull / 16f;
		angVelPitch = (float)gyroPitchFull / 16f;
		angVelRoll = (float)gyroRollFull / 16f;
		accelXG = (tempDouble = (float)accelXFull / 8192f);
		accelYG = (tempDouble = (float)accelYFull / 8192f);
		accelZG = (tempDouble = (float)accelZFull / 8192f);
		accelX = -aX / 64;
		accelY = -aY / 64;
		accelZ = aZ / 64;
		outputAccelX = 0;
		outputAccelY = 0;
		outputAccelZ = 0;
		outputGyroControls = false;
		elapsed = elapsedDelta;
		previousAxis = prevAxis;
	}
}
