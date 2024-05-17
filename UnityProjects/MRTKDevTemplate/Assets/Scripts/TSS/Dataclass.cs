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