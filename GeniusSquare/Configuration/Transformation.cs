namespace GeniusSquare.Configuration;

public enum Transformation
{
    None = 0,
    ReflectX = 1,
    ReflectY = 2,
    ReflectXY = ReflectX | ReflectY,
    Rotate = 4,
    RotateReflect = Rotate | ReflectX,
}
