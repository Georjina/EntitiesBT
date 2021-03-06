namespace EntitiesBT.Sample
{

public interface Int32Property : EntitiesBT.Variable.IVariableProperty<System.Int32> { }
public class Int32ScriptableObjectVariableProperty : EntitiesBT.Variable.ScriptableObjectVariableProperty<System.Int32>, Int32Property { }
public class Int32NodeVariableProperty : EntitiesBT.Variable.NodeVariableProperty<System.Int32>, Int32Property { }
public class Int32ComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<System.Int32>, Int32Property { }
public class Int32CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Int32>, Int32Property { }

public interface Int64Property : EntitiesBT.Variable.IVariableProperty<System.Int64> { }
public class Int64ScriptableObjectVariableProperty : EntitiesBT.Variable.ScriptableObjectVariableProperty<System.Int64>, Int64Property { }
public class Int64NodeVariableProperty : EntitiesBT.Variable.NodeVariableProperty<System.Int64>, Int64Property { }
public class Int64ComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<System.Int64>, Int64Property { }
public class Int64CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Int64>, Int64Property { }

public interface SingleProperty : EntitiesBT.Variable.IVariableProperty<System.Single> { }
public class SingleScriptableObjectVariableProperty : EntitiesBT.Variable.ScriptableObjectVariableProperty<System.Single>, SingleProperty { }
public class SingleNodeVariableProperty : EntitiesBT.Variable.NodeVariableProperty<System.Single>, SingleProperty { }
public class SingleComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<System.Single>, SingleProperty { }
public class SingleCustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<System.Single>, SingleProperty { }

public interface float2Property : EntitiesBT.Variable.IVariableProperty<Unity.Mathematics.float2> { }
public class float2ScriptableObjectVariableProperty : EntitiesBT.Variable.ScriptableObjectVariableProperty<Unity.Mathematics.float2>, float2Property { }
public class float2NodeVariableProperty : EntitiesBT.Variable.NodeVariableProperty<Unity.Mathematics.float2>, float2Property { }
public class float2ComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<Unity.Mathematics.float2>, float2Property { }
public class float2CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<Unity.Mathematics.float2>, float2Property { }

public interface float3Property : EntitiesBT.Variable.IVariableProperty<Unity.Mathematics.float3> { }
public class float3ScriptableObjectVariableProperty : EntitiesBT.Variable.ScriptableObjectVariableProperty<Unity.Mathematics.float3>, float3Property { }
public class float3NodeVariableProperty : EntitiesBT.Variable.NodeVariableProperty<Unity.Mathematics.float3>, float3Property { }
public class float3ComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<Unity.Mathematics.float3>, float3Property { }
public class float3CustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<Unity.Mathematics.float3>, float3Property { }

public interface quaternionProperty : EntitiesBT.Variable.IVariableProperty<Unity.Mathematics.quaternion> { }
public class quaternionScriptableObjectVariableProperty : EntitiesBT.Variable.ScriptableObjectVariableProperty<Unity.Mathematics.quaternion>, quaternionProperty { }
public class quaternionNodeVariableProperty : EntitiesBT.Variable.NodeVariableProperty<Unity.Mathematics.quaternion>, quaternionProperty { }
public class quaternionComponentVariableProperty : EntitiesBT.Variable.ComponentVariableProperty<Unity.Mathematics.quaternion>, quaternionProperty { }
public class quaternionCustomVariableProperty : EntitiesBT.Variable.CustomVariableProperty<Unity.Mathematics.quaternion>, quaternionProperty { }


}

