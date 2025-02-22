namespace NekoBoiNick.FFXIV.DalamudPlugin.HousingTimeoutReminder.Extensions;

public struct Vector2 : IEquatable<Vector2> {
  public float X { get; set; }
  public float Y { get; set; }

  public static Vector2 Zero => new Vector2(0);

  /// <summary>
  /// Internal method to wrap all constructors into one instance.
  /// </summary>
  /// <param name="xUnit">The x coordinate.</param>
  /// <param name="yUnit">The y coordinate.</param>
  /// <param name="_">A throwaway parameter to make this constructor unique.</param>
  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private Vector2(float xUnit, float yUnit, bool _) {
    this.X = xUnit;
    this.Y = yUnit;
  }

  /// <summary>
  /// Creates a new <see cref="Vector2"/>.
  /// </summary>
  /// <param name="xUnit">The x coordinate.</param>
  /// <param name="yUnit">The y coordinate.</param>
  public Vector2(float xUnit, float yUnit) : this(xUnit: xUnit, yUnit: yUnit, false) { }

  /// <inheritdoc cref="Extensions.Vector2(float, float)"/>
  public Vector2(int xUnit, int yUnit) : this(xUnit: xUnit, yUnit: yUnit, false) { }

  /// <inheritdoc cref="Extensions.Vector2(float, float)"/>
  public Vector2(long xUnit, long yUnit) : this(xUnit: xUnit, yUnit: yUnit, false) { }

  /// <inheritdoc cref="Extensions.Vector2(float, float)"/>
  public Vector2(float value) : this(value, value, false) { }

  /// <inheritdoc cref="Extensions.Vector2(float, float)"/>
  public Vector2(long value) : this(value, value, false) { }

  /// <inheritdoc cref="Extensions.Vector2(float, float)"/>
  public Vector2(int value) : this(value, value, false) { }

  /// <summary>
  /// Indicates whether this instance and a specified instance of <see cref="Vector2" /> are equal.
  /// </summary>
  /// <param name="other">The <see cref="Vector2" /> to compare to.</param>
  /// <returns><see langworld="true" /> if the instances are equal; otherwise <see langworld="false" />.</returns>
  public bool Equals(Vector2 other)
    => this.X.Equals(other.X)
       && this.X.Equals(other.X);

  /// <summary>
  /// Indicates whether this instance and a specified instance of nullable <see cref="Vector2" /> are equal.
  /// </summary>
  /// <param name="other">The <see cref="Vector2" /> to compare to.</param>
  /// <returns><see langworld="true" /> if the instances are equal; otherwise <see langworld="false" />.</returns>
  public bool Equals([NotNullWhen(true)] Vector2? other)
    => other.HasValue
       && this.X.Equals(other.Value.X)
       && this.X.Equals(other.Value.X);

  /// <inheritdoc />
  public override bool Equals([NotNullWhen(true)] object? obj)
    => obj is Vector2 other
       && this.Equals(other: other);

#region Self Operators
  /// <summary>
  /// Multiplies two instances.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns>A new instance with the <paramref name="left" /> and <paramref name="right" /> values multiplied.</returns>
  public static Vector2 operator *(Vector2 left, Vector2 right)
    => new Vector2(left.X * right.X, left.Y * right.Y);

  /// <summary>
  /// Adds two instances.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns>A new instance with the <paramref name="left" /> and <paramref name="right" /> values added.</returns>
  public static Vector2 operator +(Vector2 left, Vector2 right)
    => new Vector2(left.X + right.X, left.Y + right.Y);

  /// <summary>
  /// Divides two instances.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns>A new instance with the <paramref name="left" /> and <paramref name="right" /> values divided.</returns>
  public static Vector2 operator /(Vector2 left, Vector2 right)
    => new Vector2(left.X / right.X, left.Y / right.Y);

  /// <summary>
  /// Subtracts two instances.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns>A new instance with the <paramref name="left" /> and <paramref name="right" /> values subtracted.</returns>
  public static Vector2 operator -(Vector2 left, Vector2 right)
    => new Vector2(left.X - right.X, left.Y - right.Y);

  /// <summary>
  /// Finds the remainder of two instances.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns>A new instance with the <paramref name="left" /> and <paramref name="right" /> values remainder.</returns>
  public static Vector2 operator %(Vector2 left, Vector2 right)
    => new Vector2(left.X % right.X, left.Y % right.Y);

  /// <summary>
  /// Finds the exponent of two instances.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns>A new instance with the <paramref name="left" /> and <paramref name="right" /> values exponent.</returns>
  public static Vector2 operator ^(Vector2 left, Vector2 right)
    => new Vector2((long)left.X ^ (long)right.X, (long)left.Y ^ (long)right.Y);

  /// <summary>
  /// Compares two instances for equality.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator ==(Vector2 left, Vector2 right)
    => left.Equals(right);

  /// <summary>
  /// Compares two instances for inequality.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left does not equal the right; <see langword="false" /> otherwise.</returns>
  public static bool operator !=(Vector2 left, Vector2 right)
    => !(left == right);

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is greater than the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator >(Vector2 left, Vector2 right)
    => left.X > right.X && left.Y > right.Y;

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is greater than or equal to the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator >=(Vector2 left, Vector2 right)
    => !(left < right);

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is less than the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator <(Vector2 left, Vector2 right)
    => left.X < right.X && left.Y < right.Y;

  /// <summary>
  /// Compares two instances to determine if the <paramref name="left"/> is less than or equal to the <paramref name="right"/>.
  /// </summary>
  /// <param name="left">The first instance.</param>
  /// <param name="right">The second instance.</param>
  /// <returns><see langword="true" />, if left equals right; <see langword="false" /> otherwise.</returns>
  public static bool operator <=(Vector2 left, Vector2 right)
    => !(left > right);
#endregion Self Operators
#region System.Numerics.Vector2 Operators
#region Left
  /// <inheritdoc cref="op_Multiply(Vector2,Vector2)"/>
  public static Vector2 operator *(NumericsVector2 left, Vector2 right)
    => new Vector2(left.X * right.X, left.Y * right.Y);

  /// <inheritdoc cref="op_Addition(Vector2,Vector2)"/>
  public static Vector2 operator +(NumericsVector2 left, Vector2 right)
    => new Vector2(left.X + right.X, left.Y + right.Y);

  /// <inheritdoc cref="op_Division(Vector2,Vector2)"/>
  public static Vector2 operator /(NumericsVector2 left, Vector2 right)
    => new Vector2(left.X / right.X, left.Y / right.Y);

  /// <inheritdoc cref="op_Subtraction(Vector2,Vector2)"/>
  public static Vector2 operator -(NumericsVector2 left, Vector2 right)
    => new Vector2(left.X - right.X, left.Y - right.Y);

  /// <inheritdoc cref="op_Modulus(Vector2,Vector2)"/>
  public static Vector2 operator %(NumericsVector2 left, Vector2 right)
    => new Vector2(left.X % right.X, left.Y % right.Y);

  /// <inheritdoc cref="op_ExclusiveOr(Vector2,Vector2)"/>
  public static Vector2 operator ^(NumericsVector2 left, Vector2 right)
    => new Vector2((long)left.X ^ (long)right.X, (long)left.Y ^ (long)right.Y);

  /// <inheritdoc cref="op_Equality(Vector2,Vector2)"/>
  public static bool operator ==(NumericsVector2 left, Vector2 right)
    => left.Equals(right);

  /// <inheritdoc cref="op_Inequality(Vector2,Vector2)"/>
  public static bool operator !=(NumericsVector2 left, Vector2 right)
    => !(left == right);

  /// <inheritdoc cref="op_GreaterThan(Vector2,Vector2)"/>
  public static bool operator >(NumericsVector2 left, Vector2 right)
    => left.X > right.X && left.Y > right.Y;

  /// <inheritdoc cref="op_GreaterThanOrEqual(Vector2,Vector2)"/>
  public static bool operator >=(NumericsVector2 left, Vector2 right)
    => !(left < right);

  /// <inheritdoc cref="op_LessThan(Vector2,Vector2)"/>
  public static bool operator <(NumericsVector2 left, Vector2 right)
    => left.X < right.X && left.Y < right.Y;

  /// <inheritdoc cref="op_LessThanOrEqual(Vector2,Vector2)"/>
  public static bool operator <=(NumericsVector2 left, Vector2 right)
    => !(left > right);
#endregion Left
#region Right
  /// <inheritdoc cref="op_Multiply(Vector2,Vector2)"/>
  public static Vector2 operator *(Vector2 left, NumericsVector2 right)
    => new Vector2(left.X * right.X, left.Y * right.Y);

  /// <inheritdoc cref="op_Addition(Vector2,Vector2)"/>
  public static Vector2 operator +(Vector2 left, NumericsVector2 right)
    => new Vector2(left.X + right.X, left.Y + right.Y);

  /// <inheritdoc cref="op_Division(Vector2,Vector2)"/>
  public static Vector2 operator /(Vector2 left, NumericsVector2 right)
    => new Vector2(left.X / right.X, left.Y / right.Y);

  /// <inheritdoc cref="op_Subtraction(Vector2,Vector2)"/>
  public static Vector2 operator -(Vector2 left, NumericsVector2 right)
    => new Vector2(left.X - right.X, left.Y - right.Y);

  /// <inheritdoc cref="op_Modulus(Vector2,Vector2)"/>
  public static Vector2 operator %(Vector2 left, NumericsVector2 right)
    => new Vector2(left.X % right.X, left.Y % right.Y);

  /// <inheritdoc cref="op_ExclusiveOr(Vector2,Vector2)"/>
  public static Vector2 operator ^(Vector2 left, NumericsVector2 right)
    => new Vector2((long)left.X ^ (long)right.X, (long)left.Y ^ (long)right.Y);

  /// <inheritdoc cref="op_Equality(Vector2,Vector2)"/>
  public static bool operator ==(Vector2 left, NumericsVector2 right)
    => left.Equals(right);

  /// <inheritdoc cref="op_Inequality(Vector2,Vector2)"/>
  public static bool operator !=(Vector2 left, NumericsVector2 right)
    => !(left == right);

  /// <inheritdoc cref="op_GreaterThan(Vector2,Vector2)"/>
  public static bool operator >(Vector2 left, NumericsVector2 right)
    => left.X > right.X && left.Y > right.Y;

  /// <inheritdoc cref="op_GreaterThanOrEqual(Vector2,Vector2)"/>
  public static bool operator >=(Vector2 left, NumericsVector2 right)
    => !(left < right);

  /// <inheritdoc cref="op_LessThan(Vector2,Vector2)"/>
  public static bool operator <(Vector2 left, NumericsVector2 right)
    => left.X < right.X && left.Y < right.Y;

  /// <inheritdoc cref="op_LessThanOrEqual(Vector2,Vector2)"/>
  public static bool operator <=(Vector2 left, NumericsVector2 right)
    => !(left > right);
#endregion Right
#endregion System.Numerics.Vector2 Operators
#region FFXIVClientStructs.FFXIV.Common.Math.Vector2 Operators
#region Left
  /// <inheritdoc cref="op_Multiply(Vector2,Vector2)"/>
  public static Vector2 operator *(CSVector2 left, Vector2 right)
    => new Vector2(left.X * right.X, left.Y * right.Y);

  /// <inheritdoc cref="op_Addition(Vector2,Vector2)"/>
  public static Vector2 operator +(CSVector2 left, Vector2 right)
    => new Vector2(left.X + right.X, left.Y + right.Y);

  /// <inheritdoc cref="op_Division(Vector2,Vector2)"/>
  public static Vector2 operator /(CSVector2 left, Vector2 right)
    => new Vector2(left.X / right.X, left.Y / right.Y);

  /// <inheritdoc cref="op_Subtraction(Vector2,Vector2)"/>
  public static Vector2 operator -(CSVector2 left, Vector2 right)
    => new Vector2(left.X - right.X, left.Y - right.Y);

  /// <inheritdoc cref="op_Modulus(Vector2,Vector2)"/>
  public static Vector2 operator %(CSVector2 left, Vector2 right)
    => new Vector2(left.X % right.X, left.Y % right.Y);

  /// <inheritdoc cref="op_ExclusiveOr(Vector2,Vector2)"/>
  public static Vector2 operator ^(CSVector2 left, Vector2 right)
    => new Vector2((long)left.X ^ (long)right.X, (long)left.Y ^ (long)right.Y);

  /// <inheritdoc cref="op_Equality(Vector2,Vector2)"/>
  public static bool operator ==(CSVector2 left, Vector2 right)
    => left.Equals(right);

  /// <inheritdoc cref="op_Inequality(Vector2,Vector2)"/>
  public static bool operator !=(CSVector2 left, Vector2 right)
    => !(left == right);

  /// <inheritdoc cref="op_GreaterThan(Vector2,Vector2)"/>
  public static bool operator >(CSVector2 left, Vector2 right)
    => left.X > right.X && left.Y > right.Y;

  /// <inheritdoc cref="op_GreaterThanOrEqual(Vector2,Vector2)"/>
  public static bool operator >=(CSVector2 left, Vector2 right)
    => !(left < right);

  /// <inheritdoc cref="op_LessThan(Vector2,Vector2)"/>
  public static bool operator <(CSVector2 left, Vector2 right)
    => left.X < right.X && left.Y < right.Y;

  /// <inheritdoc cref="op_LessThanOrEqual(Vector2,Vector2)"/>
  public static bool operator <=(CSVector2 left, Vector2 right)
    => !(left > right);
#endregion Left
#region Right
  /// <inheritdoc cref="op_Multiply(Vector2,Vector2)"/>
  public static Vector2 operator *(Vector2 left, CSVector2 right)
    => new Vector2(left.X * right.X, left.Y * right.Y);

  /// <inheritdoc cref="op_Addition(Vector2,Vector2)"/>
  public static Vector2 operator +(Vector2 left, CSVector2 right)
    => new Vector2(left.X + right.X, left.Y + right.Y);

  /// <inheritdoc cref="op_Division(Vector2,Vector2)"/>
  public static Vector2 operator /(Vector2 left, CSVector2 right)
    => new Vector2(left.X / right.X, left.Y / right.Y);

  /// <inheritdoc cref="op_Subtraction(Vector2,Vector2)"/>
  public static Vector2 operator -(Vector2 left, CSVector2 right)
    => new Vector2(left.X - right.X, left.Y - right.Y);

  /// <inheritdoc cref="op_Modulus(Vector2,Vector2)"/>
  public static Vector2 operator %(Vector2 left, CSVector2 right)
    => new Vector2(left.X % right.X, left.Y % right.Y);

  /// <inheritdoc cref="op_ExclusiveOr(Vector2,Vector2)"/>
  public static Vector2 operator ^(Vector2 left, CSVector2 right)
    => new Vector2((long)left.X ^ (long)right.X, (long)left.Y ^ (long)right.Y);

  /// <inheritdoc cref="op_Equality(Vector2,Vector2)"/>
  public static bool operator ==(Vector2 left, CSVector2 right)
    => left.Equals(right);

  /// <inheritdoc cref="op_Inequality(Vector2,Vector2)"/>
  public static bool operator !=(Vector2 left, CSVector2 right)
    => !(left == right);

  /// <inheritdoc cref="op_GreaterThan(Vector2,Vector2)"/>
  public static bool operator >(Vector2 left, CSVector2 right)
    => left.X > right.X && left.Y > right.Y;

  /// <inheritdoc cref="op_GreaterThanOrEqual(Vector2,Vector2)"/>
  public static bool operator >=(Vector2 left, CSVector2 right)
    => !(left < right);

  /// <inheritdoc cref="op_LessThan(Vector2,Vector2)"/>
  public static bool operator <(Vector2 left, CSVector2 right)
    => left.X < right.X && left.Y < right.Y;

  /// <inheritdoc cref="op_LessThanOrEqual(Vector2,Vector2)"/>
  public static bool operator <=(Vector2 left, CSVector2 right)
    => !(left > right);
#endregion Right
#endregion FFXIVClientStructs.FFXIV.Common.Math.Vector2 Operators
#region Float Operators
#region Left
  /// <inheritdoc cref="op_Multiply(Vector2,Vector2)"/>
  public static Vector2 operator *(float left, Vector2 right)
    => new Vector2(left * right.X, left * right.Y);

  /// <inheritdoc cref="op_Addition(Vector2,Vector2)"/>
  public static Vector2 operator +(float left, Vector2 right)
    => new Vector2(left + right.X, left + right.Y);

  /// <inheritdoc cref="op_Division(Vector2,Vector2)"/>
  public static Vector2 operator /(float left, Vector2 right)
    => new Vector2(left / right.X, left / right.Y);

  /// <inheritdoc cref="op_Subtraction(Vector2,Vector2)"/>
  public static Vector2 operator -(float left, Vector2 right)
    => new Vector2(left - right.X, left - right.Y);

  /// <inheritdoc cref="op_Modulus(Vector2,Vector2)"/>
  public static Vector2 operator %(float left, Vector2 right)
    => new Vector2(left % right.X, left % right.Y);

  /// <inheritdoc cref="op_ExclusiveOr(Vector2,Vector2)"/>
  public static Vector2 operator ^(float left, Vector2 right)
    => new Vector2((long)left ^ (long)right.X, (long)left ^ (long)right.Y);

  /// <inheritdoc cref="op_Equality(Vector2,Vector2)"/>
  public static bool operator ==(float left, Vector2 right)
    => new Vector2(left).Equals(right);

  /// <inheritdoc cref="op_Inequality(Vector2,Vector2)"/>
  public static bool operator !=(float left, Vector2 right)
    => !(left == right);

  /// <inheritdoc cref="op_GreaterThan(Vector2,Vector2)"/>
  public static bool operator >(float left, Vector2 right)
    => left > right.X && left > right.Y;

  /// <inheritdoc cref="op_GreaterThanOrEqual(Vector2,Vector2)"/>
  public static bool operator >=(float left, Vector2 right)
    => !(left < right);

  /// <inheritdoc cref="op_LessThan(Vector2,Vector2)"/>
  public static bool operator <(float left, Vector2 right)
    => left < right.X && left < right.Y;

  /// <inheritdoc cref="op_LessThanOrEqual(Vector2,Vector2)"/>
  public static bool operator <=(float left, Vector2 right)
    => !(left > right);
#endregion Left
#region Right
  /// <inheritdoc cref="op_Multiply(Vector2,Vector2)"/>
  public static Vector2 operator *(Vector2 left, float right)
    => new Vector2(left.X * right, left.Y * right);

  /// <inheritdoc cref="op_Addition(Vector2,Vector2)"/>
  public static Vector2 operator +(Vector2 left, float right)
    => new Vector2(left.X + right, left.Y + right);

  /// <inheritdoc cref="op_Division(Vector2,Vector2)"/>
  public static Vector2 operator /(Vector2 left, float right)
    => new Vector2(left.X / right, left.Y / right);

  /// <inheritdoc cref="op_Subtraction(Vector2,Vector2)"/>
  public static Vector2 operator -(Vector2 left, float right)
    => new Vector2(left.X - right, left.Y - right);

  /// <inheritdoc cref="op_Modulus(Vector2,Vector2)"/>
  public static Vector2 operator %(Vector2 left, float right)
    => new Vector2(left.X % right, left.Y % right);

  /// <inheritdoc cref="op_ExclusiveOr(Vector2,Vector2)"/>
  public static Vector2 operator ^(Vector2 left, float right)
    => new Vector2((long)left.X ^ (long)right, (long)left.Y ^ (long)right);

  /// <inheritdoc cref="op_Equality(Vector2,Vector2)"/>
  public static bool operator ==(Vector2 left, float right)
    => left.Equals(new Vector2(right));

  /// <inheritdoc cref="op_Inequality(Vector2,Vector2)"/>
  public static bool operator !=(Vector2 left, float right)
    => !(left == right);

  /// <inheritdoc cref="op_GreaterThan(Vector2,Vector2)"/>
  public static bool operator >(Vector2 left, float right)
    => left.X > right && left.Y > right;

  /// <inheritdoc cref="op_GreaterThanOrEqual(Vector2,Vector2)"/>
  public static bool operator >=(Vector2 left, float right)
    => !(left < right);

  /// <inheritdoc cref="op_LessThan(Vector2,Vector2)"/>
  public static bool operator <(Vector2 left, float right)
    => left.X < right && left.Y < right;

  /// <inheritdoc cref="op_LessThanOrEqual(Vector2,Vector2)"/>
  public static bool operator <=(Vector2 left, float right)
    => !(left > right);
#endregion Right
#endregion Float Operators
#region Int Operators
#region Left
  /// <inheritdoc cref="op_Multiply(Vector2,Vector2)"/>
  public static Vector2 operator *(int left, Vector2 right)
    => new Vector2(left * right.X, left * right.Y);

  /// <inheritdoc cref="op_Addition(Vector2,Vector2)"/>
  public static Vector2 operator +(int left, Vector2 right)
    => new Vector2(left + right.X, left + right.Y);

  /// <inheritdoc cref="op_Division(Vector2,Vector2)"/>
  public static Vector2 operator /(int left, Vector2 right)
    => new Vector2(left / right.X, left / right.Y);

  /// <inheritdoc cref="op_Subtraction(Vector2,Vector2)"/>
  public static Vector2 operator -(int left, Vector2 right)
    => new Vector2(left - right.X, left - right.Y);

  /// <inheritdoc cref="op_Modulus(Vector2,Vector2)"/>
  public static Vector2 operator %(int left, Vector2 right)
    => new Vector2(left % right.X, left % right.Y);

  /// <inheritdoc cref="op_ExclusiveOr(Vector2,Vector2)"/>
  public static Vector2 operator ^(int left, Vector2 right)
    => new Vector2(left ^ (long)right.X, left ^ (long)right.Y);

  /// <inheritdoc cref="op_Equality(Vector2,Vector2)"/>
  public static bool operator ==(int left, Vector2 right)
    => new Vector2(left).Equals(right);

  /// <inheritdoc cref="op_Inequality(Vector2,Vector2)"/>
  public static bool operator !=(int left, Vector2 right)
    => !(left == right);

  /// <inheritdoc cref="op_GreaterThan(Vector2,Vector2)"/>
  public static bool operator >(int left, Vector2 right)
    => left > right.X && left > right.Y;

  /// <inheritdoc cref="op_GreaterThanOrEqual(Vector2,Vector2)"/>
  public static bool operator >=(int left, Vector2 right)
    => !(left < right);

  /// <inheritdoc cref="op_LessThan(Vector2,Vector2)"/>
  public static bool operator <(int left, Vector2 right)
    => left < right.X && left < right.Y;

  /// <inheritdoc cref="op_LessThanOrEqual(Vector2,Vector2)"/>
  public static bool operator <=(int left, Vector2 right)
    => !(left > right);
#endregion Left
#region Right
  /// <inheritdoc cref="op_Multiply(Vector2,Vector2)"/>
  public static Vector2 operator *(Vector2 left, int right)
    => new Vector2(left.X * right, left.Y * right);

  /// <inheritdoc cref="op_Addition(Vector2,Vector2)"/>
  public static Vector2 operator +(Vector2 left, int right)
    => new Vector2(left.X + right, left.Y + right);

  /// <inheritdoc cref="op_Division(Vector2,Vector2)"/>
  public static Vector2 operator /(Vector2 left, int right)
    => new Vector2(left.X / right, left.Y / right);

  /// <inheritdoc cref="op_Subtraction(Vector2,Vector2)"/>
  public static Vector2 operator -(Vector2 left, int right)
    => new Vector2(left.X - right, left.Y - right);

  /// <inheritdoc cref="op_Modulus(Vector2,Vector2)"/>
  public static Vector2 operator %(Vector2 left, int right)
    => new Vector2(left.X % right, left.Y % right);

  /// <inheritdoc cref="op_ExclusiveOr(Vector2,Vector2)"/>
  public static Vector2 operator ^(Vector2 left, int right)
    => new Vector2((long)left.X ^ right, (long)left.Y ^ right);

  /// <inheritdoc cref="op_Equality(Vector2,Vector2)"/>
  public static bool operator ==(Vector2 left, int right)
    => left.Equals(new Vector2(right));

  /// <inheritdoc cref="op_Inequality(Vector2,Vector2)"/>
  public static bool operator !=(Vector2 left, int right)
    => !(left == right);

  /// <inheritdoc cref="op_GreaterThan(Vector2,Vector2)"/>
  public static bool operator >(Vector2 left, int right)
    => left.X > right && left.Y > right;

  /// <inheritdoc cref="op_GreaterThanOrEqual(Vector2,Vector2)"/>
  public static bool operator >=(Vector2 left, int right)
    => !(left < right);

  /// <inheritdoc cref="op_LessThan(Vector2,Vector2)"/>
  public static bool operator <(Vector2 left, int right)
    => left.X < right && left.Y < right;

  /// <inheritdoc cref="op_LessThanOrEqual(Vector2,Vector2)"/>
  public static bool operator <=(Vector2 left, int right)
    => !(left > right);
#endregion Right
#endregion Int Operators
#region UInt64 Operators
#region Left
  /// <inheritdoc cref="op_Multiply(Vector2,Vector2)"/>
  public static Vector2 operator *(long left, Vector2 right)
    => new Vector2(left * right.X, left * right.Y);

  /// <inheritdoc cref="op_Addition(Vector2,Vector2)"/>
  public static Vector2 operator +(long left, Vector2 right)
    => new Vector2(left + right.X, left + right.Y);

  /// <inheritdoc cref="op_Division(Vector2,Vector2)"/>
  public static Vector2 operator /(long left, Vector2 right)
    => new Vector2(left / right.X, left / right.Y);

  /// <inheritdoc cref="op_Subtraction(Vector2,Vector2)"/>
  public static Vector2 operator -(long left, Vector2 right)
    => new Vector2(left - right.X, left - right.Y);

  /// <inheritdoc cref="op_Modulus(Vector2,Vector2)"/>
  public static Vector2 operator %(long left, Vector2 right)
    => new Vector2(left % right.X, left % right.Y);

  /// <inheritdoc cref="op_ExclusiveOr(Vector2,Vector2)"/>
  public static Vector2 operator ^(long left, Vector2 right)
    => new Vector2(left ^ (long)right.X, left ^ (long)right.Y);

  /// <inheritdoc cref="op_Equality(Vector2,Vector2)"/>
  public static bool operator ==(long left, Vector2 right)
    => new Vector2(left).Equals(right);

  /// <inheritdoc cref="op_Inequality(Vector2,Vector2)"/>
  public static bool operator !=(long left, Vector2 right)
    => !(left == right);

  /// <inheritdoc cref="op_GreaterThan(Vector2,Vector2)"/>
  public static bool operator >(long left, Vector2 right)
    => left > right.X && left > right.Y;

  /// <inheritdoc cref="op_GreaterThanOrEqual(Vector2,Vector2)"/>
  public static bool operator >=(long left, Vector2 right)
    => !(left < right);

  /// <inheritdoc cref="op_LessThan(Vector2,Vector2)"/>
  public static bool operator <(long left, Vector2 right)
    => left < right.X && left < right.Y;

  /// <inheritdoc cref="op_LessThanOrEqual(Vector2,Vector2)"/>
  public static bool operator <=(long left, Vector2 right)
    => !(left > right);
#endregion Left
#region Right
  /// <inheritdoc cref="op_Multiply(Vector2,Vector2)"/>
  public static Vector2 operator *(Vector2 left, long right)
    => new Vector2(left.X * right, left.Y * right);

  /// <inheritdoc cref="op_Addition(Vector2,Vector2)"/>
  public static Vector2 operator +(Vector2 left, long right)
    => new Vector2(left.X + right, left.Y + right);

  /// <inheritdoc cref="op_Division(Vector2,Vector2)"/>
  public static Vector2 operator /(Vector2 left, long right)
    => new Vector2(left.X / right, left.Y / right);

  /// <inheritdoc cref="op_Subtraction(Vector2,Vector2)"/>
  public static Vector2 operator -(Vector2 left, long right)
    => new Vector2(left.X - right, left.Y - right);

  /// <inheritdoc cref="op_Modulus(Vector2,Vector2)"/>
  public static Vector2 operator %(Vector2 left, long right)
    => new Vector2(left.X % right, left.Y % right);

  /// <inheritdoc cref="op_ExclusiveOr(Vector2,Vector2)"/>
  public static Vector2 operator ^(Vector2 left, long right)
    => new Vector2((long)left.X ^ right, (long)left.Y ^ right);

  /// <inheritdoc cref="op_Equality(Vector2,Vector2)"/>
  public static bool operator ==(Vector2 left, long right)
    => left.Equals(new Vector2(right));

  /// <inheritdoc cref="op_Inequality(Vector2,Vector2)"/>
  public static bool operator !=(Vector2 left, long right)
    => !(left == right);

  /// <inheritdoc cref="op_GreaterThan(Vector2,Vector2)"/>
  public static bool operator >(Vector2 left, long right)
    => left.X > right && left.Y > right;

  /// <inheritdoc cref="op_GreaterThanOrEqual(Vector2,Vector2)"/>
  public static bool operator >=(Vector2 left, long right)
    => !(left < right);

  /// <inheritdoc cref="op_LessThan(Vector2,Vector2)"/>
  public static bool operator <(Vector2 left, long right)
    => left.X < right && left.Y < right;

  /// <inheritdoc cref="op_LessThanOrEqual(Vector2,Vector2)"/>
  public static bool operator <=(Vector2 left, long right)
    => !(left > right);
#endregion Right
#endregion UInt64 Operators


#region Implicit Conversions
  /// <summary>
  /// Converts this instance of <see cref="Vector2"/> to a <see cref="System.Numerics.Vector2"/>.
  /// </summary>
  private NumericsVector2 ToNumericsVector2()
    => new NumericsVector2(this.X, this.Y);

  /// <summary>
  /// Converts this instance of <see cref="Vector2"/> to a <see cref="FFXIVClientStructs.FFXIV.Common.Math.Vector2"/>.
  /// </summary>
  private CSVector2 ToCSVector2()
    => new CSVector2(this.X, this.Y);

  /// <summary>
  /// Converts an instance of <see cref="System.Numerics.Vector2"/> to an instance of <see cref="Vector2"/>.
  /// </summary>
  private static Vector2 ToVector2(NumericsVector2 value)
    => new Vector2(value.X, value.Y);

  /// <summary>
  /// Converts an instance of <see cref="FFXIVClientStructs.FFXIV.Common.Math.Vector2"/> to an instance of <see cref="Vector2"/>.
  /// </summary>
  private static Vector2 ToVector2(CSVector2 value)
    => new Vector2(value.X, value.Y);

  /// <inheritdoc cref="ToNumericsVector2" />
  public static implicit operator NumericsVector2(Vector2 vector)
    => vector.ToNumericsVector2();

  /// <inheritdoc cref="ToCSVector2" />
  public static implicit operator CSVector2(Vector2 vector)
    => vector.ToCSVector2();

  /// <inheritdoc cref="ToVector2(NumericsVector2)" />
  public static implicit operator Vector2(NumericsVector2 vector)
    => ToVector2(vector);

  /// <inheritdoc cref="ToVector2(CSVector2)" />
  public static implicit operator Vector2(CSVector2 vector)
    => ToVector2(vector);
#endregion Implicit Conversions

  /// <inheritdoc />
  [SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
  public override int GetHashCode() => base.GetHashCode();
}