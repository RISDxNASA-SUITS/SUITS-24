using Newtonsoft.Json;

public record UIAWrapper
{
    public UIA uia;
}

public record UIA
{
    // External Device sends Switch Values to backend
    public bool eva1_power;
    public bool eva1_oxy;
    public bool eva1_water_waste;
    public bool eva1_water_supply;

    public bool eva2_power;
    public bool eva2_oxy;
    public bool eva2_water_waste;
    public bool eva2_water_supply;

    public bool oxy_vent;
    public bool depress;
};

public record DCUWrapper
{
    public DCU dcu;
}

public record DCU
{
    public DCU_EVA eva1;
    public DCU_EVA eva2;
}

public record DCU_EVA
{
    // EVA Switches
    public bool batt;
    public bool oxy;
    public bool comm;
    public bool fan;
    public bool pump;
    public bool co2;
}

public record SPECWrapper
{
    public SPEC spec;
}

public record SPEC
{
    public SPEC_EVA eva1;
    public SPEC_EVA eva2;
}

public record SPEC_EVA
{
    public string name;
    public int id;
    public SPEC_DATA data;
}

public record SPEC_DATA
{
    public float SiO2;
    public float TiO2;
    public float Al2O3;
    public float FeO;
    public float MnO;
    public float MgO;
    public float CaO;
    public float K2O;
    public float P2O3;
    public float other;
}

public record IMUWrapper
{
    public IMU imu;
}

public record IMU
{
    public IMU_DATA eva1;
    public IMU_DATA eva2;
}

public record IMU_DATA
{
    public float posx;
    public float posy;
    public float heading;
}

public record ERRORWrapper
{
    public ERROR error;
}

public record ERROR
{
    public bool fan_error;
    public bool oxy_error;
    public bool pump_error;
}