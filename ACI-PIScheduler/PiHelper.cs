using eRCM_Kernel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACI_PIScheduler
{
    class PiHelper
    {
    }
    public class Service
    {
        public string UnitName { get; set; }
        public string ViewerFileName { get; set; }
        public string ServiceName { get; set; }
        public Tag EngineSpeed { get; set; }
        public Tag DischargePressure { get; set; }
        public Pressure StageSuctionPressure { get; set; }
        // public Pressure StageDischargePressure { get; set; }
        public Temperature StageSuctionTemperature { get; set; }
        public int LoadStep { get; set; }
        public List<Stages> Stages { get; set; }
    }
    public class Services
    {
        public List<Service> ServiceList { get; set; }
    }
    public class Stages
    {
        public List<int> CylinderSet { get; set; }
        public Tag Pd { get; set; }
        public int Stage { get; set; }
    }
    public class Pressure
    {
        public Tag Ps1 { get; set; }
        public Tag Ps2 { get; set; }
    }
    public class Temperature
    {
        private Tag ts1;
        private Tag ts2;

        private Tag ts3;
        private Tag ts4;
        private Tag ts5;
        private Tag ts6;
        public Tag Ts3 { get => ts3; set => ts3 = value; }
        public Tag Ts4 { get => ts4; set => ts4 = value; }
        public Tag Ts5 { get => ts5; set => ts5 = value; }
        public Tag Ts6 { get => ts6; set => ts6 = value; }
        public Tag Ts2 { get => ts2; set => ts2 = value; }
        public Tag Ts1 { get => ts1; set => ts1 = value; }
    }
    public class Tag
    {
        private string name;
        private object value;
        public string Name { get => name; set => name = value; }
        public object Value { get => value; set => this.value = value; }
    }
    public class AciModelInputs
    {
        public double PsG { get; set; }
        public double PdG { get; set; }
        public double Rpm { get; set; }
        public double Ts1 { get; set; }
        public double Ts2 { get; set; }
        public double Ts3 { get; set; }
        public double Ts4 { get; set; }
        public double Ts5 { get; set; }
        public double Ts6 { get; set; }
        public string ErrorMsg { get; set; }


        public AciModelInputs(Service unit)
        {
            try
            {
                PsG = Convert.ToDouble(unit.StageSuctionPressure.Ps1.Value);
                PdG = Convert.ToDouble(unit.DischargePressure.Value);
                this.Rpm = Convert.ToDouble(unit.EngineSpeed.Value);
                Ts1 = Convert.ToDouble(unit.StageSuctionTemperature.Ts1.Value);
               // Ts2 = Convert.ToDouble(unit.StageSuctionTemperature.Ts2.Value);
                if (unit.Stages.Count() > 1)
                {
                    if (unit.StageSuctionTemperature.Ts2 != null)
                        Ts2 = Convert.ToDouble(unit.StageSuctionTemperature.Ts2.Value);
                    else
                        Ts2 = 0;
                    if (unit.StageSuctionTemperature.Ts3 != null)
                        Ts3 = Convert.ToDouble(unit.StageSuctionTemperature.Ts3.Value);
                    else
                        Ts3 = 0;

                    if (unit.StageSuctionTemperature.Ts4 != null)
                        Ts4 = Convert.ToDouble(unit.StageSuctionTemperature.Ts4.Value);
                    else
                        Ts4 = 0;
                    if (unit.StageSuctionTemperature.Ts5 != null)
                        Ts5 = Convert.ToDouble(unit.StageSuctionTemperature.Ts5.Value);
                    else
                        Ts5 = 0;
                    if (unit.StageSuctionTemperature.Ts6 != null)
                        Ts6 = Convert.ToDouble(unit.StageSuctionTemperature.Ts6.Value);
                    else
                        Ts6 = 0;
                }
                else
                {
                    Ts3 = Ts4 = Ts5 = Ts6 = 0;
                }

                //Trace.TraceInformation("Ps1: "+unit.StageSuctionPressure.Ps1.Value.ToString());
                //Trace.TraceInformation("Ps2: " + unit.StageSuctionPressure.Ps1.Value.ToString());
                //Trace.TraceInformation("PdF: " + unit.DischargePressure.Value.ToString());
                //Trace.TraceInformation("RPM: " + unit.EngineSpeed.Value.ToString());
                //Trace.TraceInformation("Ts1: " + unit.StageSuctionTemperature.Ts1.Value.ToString());
                //Trace.TraceInformation("Ts2: " + unit.StageSuctionTemperature.Ts2.Value.ToString());
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                Trace.TraceError(ex.Message);
                Trace.TraceError("Ps1: " + unit.StageSuctionPressure.Ps1.Value.ToString());
                Trace.TraceError("Ps2: " + unit.StageSuctionPressure.Ps1.Value.ToString());
                Trace.TraceError("PdF: " + unit.DischargePressure.Value.ToString());
                Trace.TraceError("RPM: " + unit.EngineSpeed.Value.ToString());
                Trace.TraceError("Ts1: " + unit.StageSuctionTemperature.Ts1.Value.ToString());
                Trace.TraceError("Ts2: " + unit.StageSuctionTemperature.Ts2.Value.ToString());
                PsG = 0;
                PdG = 0;
                this.Rpm = 0;
                Ts1 = 0;
                Ts2 = 0;
                Ts3 = Ts4 = Ts5 = Ts6 = 0;
            }
        }


    }
    //All Outputs
    public class Outputs
    {
        public string UnitName { get; set; }
        public string ServiceName { get; set; }
        public LoadSteps LoadStep { get; set; }
        public LoadPrediction LoadPrediction { get; set; }
        public FlowPrediction FlowPrediction { get; set; }
        public FuelRates FuelRate { get; set; }
        public Tag MechanicalEfficiency { get; set; }
        public Tag CurrentTorque { get; set; }
        public Tag IdealTorque { get; set; }
        public Tag MaxLoad { get; set; }
        public Tag ErrorCode { get; set; }
        public List<StageInfo> StageInfo { get; set; }
        public List<LoadBalance> LoadBalance { get; set; }
        public List<HeadEndInfo> HeadEndInfo { get; set; }
        public List<CrankEndInfo> CrankEndInfo { get; set; }
        public List<CompressionForces> CompressionForces { get; set; }
        public List<TensionForces> TensionForces { get; set; }

        public Tag RatedLoad { get; set; }
        public Tag RatedSpeed { get; set; }
        public Tag MinChangeAllowed { get; set; }
        public Tag MaxChangeAllowed { get; set; }
        public Tag MinSpeed { get; set; }
        public Tag MaxSpeed { get; set; }
        public Tag MaxAllowedDischargePressure { get; set; }
        public Tag IdealFlow90T { get; set; }
        public Tag IdealLoad90T { get; set; }
        public Tag IdealFuelRate90T { get; set; }
        public Tag IdealLoadStep90T { get; set; }
        public Tag IdealLoadStepDetail90T { get; set; }
        public Tag IdealFlow95T { get; set; }
        public Tag IdealLoad95T { get; set; }
        public Tag IdealFuelRate95T { get; set; }
        public Tag IdealLoadStep95T { get; set; }
        public Tag IdealLoadStepDetail95T { get; set; }

    }

    public class IdealOutputAtGivenTorque
    {
        public double IdealFlowAtGivenTorque { get; set; }
        public double IdealLoadAtGivenTorque { get; set; }
        public double IdealFuelRateAtGivenTorque { get; set; }
        public double IdealLoadStepAtGivenTorque { get; set; }
        public string IdealLoadStepDetailAtGivenTorque { get; set; }

    }

    public class CurrentOutputAtGivenLoadStep
    {
        public double CurrentFlow { get; set; }
        public double CurrentLoad { get; set; }
        public double CurrentFuelRate { get; set; }
        public double CurrentLoadStep { get; set; }
        public string CurrentLoadStepDetail { get; set; }

    }
    public class OutputList
    {
        public List<Outputs> Outputs { get; set; }
    }
    public class LoadSteps
    {
        public Tag Current { get; set; }
        public Tag CurrentDetails { get; set; }
        public Tag Ideal { get; set; }
        public Tag IdealDetails { get; set; }
        public Tag NextUp { get; set; }
        public Tag NextDown { get; set; }

        public static LoadSteps GetLoadSteps(eRCM_KernelControl objACIkernal, LoadSteps objUnitLS)
        {
            var result = objUnitLS;
            result.Current.Value = objACIkernal.CurrentLoadStep;
            result.CurrentDetails.Value = objACIkernal.LoadStepName(objACIkernal.CurrentLoadStep);
            result.Ideal.Value = objACIkernal.FindOptimalLoadStep(1, 0, 0);
            result.IdealDetails.Value = objACIkernal.LoadStepName(objACIkernal.FindOptimalLoadStep(1, 0, 0));
            result.NextDown.Value = objACIkernal.NextLoadStepDown();
            result.NextUp.Value = objACIkernal.NextLoadStepUp();
            return result;
        }

    }
    public class LoadPrediction
    {
        public Tag Current { get; set; }
        public Tag Ideal { get; set; }

        public static LoadPrediction GetLoadPrediction(eRCM_KernelControl objACIkernal, LoadPrediction objUnitLP)
        {
            objUnitLP.Current.Value = objACIkernal.LoadArray(objACIkernal.CurrentLoadStep);
            objUnitLP.Ideal.Value = objACIkernal.LoadArray(objACIkernal.FindOptimalLoadStep(1, 0, 0));
            return objUnitLP;
        }
    }
    public class FlowPrediction
    {
        public Tag Current { get; set; }
        public Tag Ideal { get; set; }

        public static FlowPrediction GetFlowPrediction(eRCM_KernelControl objACIkernal, FlowPrediction objUnitFP)
        {
            objUnitFP.Current.Value = objACIkernal.FlowArray(objACIkernal.CurrentLoadStep);
            objUnitFP.Ideal.Value = objACIkernal.FlowArray(objACIkernal.FindOptimalLoadStep(1, 0, 0));

            return objUnitFP;
        }
    }
    public class FuelRates
    {
        public Tag Current { get; set; }
        public Tag Ideal { get; set; }

        public static FuelRates GetFuelRates(eRCM_KernelControl objACIkernal, FuelRates objUnitFR, Service unit)
        {
            //Get Fuel Rate in Current Load Step
            objUnitFR.Current.Value = objACIkernal.EngineDataFuelRate();
            //Reset ACI Object with Ideal Load Step
            AciModelInputs objAciInput = new AciModelInputs(unit);
            objACIkernal.CurrentLoadStep = objACIkernal.FindOptimalLoadStep(1, 0, 0);
            objACIkernal.ChangeOpCondition(objAciInput.PsG, objAciInput.PdG, objAciInput.Rpm, objAciInput.Ts1, objAciInput.Ts2, objAciInput.Ts3,
                objAciInput.Ts4, objAciInput.Ts5, objAciInput.Ts6);
            //Get Fuel rate in Ideal Load Step
            objUnitFR.Ideal.Value = objACIkernal.EngineDataFuelRate();
            return objUnitFR;
        }
    }
    public class StageInfo
    {
        public int StageNumber { get; set; }
        public Tag DischargeTemperature { get; set; }
        public Tag CompressionRatio { get; set; }
        public Tag DischargePressureAtGadge { get; set; }
        public Tag DischargePressureAtFlange { get; set; }
        public Tag Load { get; set; }
        public Tag RatioActualVsTheoriticalPressure { get; set; }
        public Tag RatioActualVsTheoriticalTemperature { get; set; }

        public static List<StageInfo> GetStageInfoList(eRCM_KernelControl objACIkernal, List<StageInfo> lstStageInfo, Service unit)
        {

            foreach (var stage in lstStageInfo)
            {
                stage.DischargeTemperature.Value = objACIkernal.StageInfo(stage.StageNumber, 23);
                stage.CompressionRatio.Value = objACIkernal.StageInfo(stage.StageNumber, 8);
                stage.DischargePressureAtGadge.Value = objACIkernal.StageInfo(stage.StageNumber, 2);
                stage.DischargePressureAtFlange.Value = objACIkernal.StageInfo(stage.StageNumber, 10);
                stage.Load.Value = objACIkernal.StageInfo(stage.StageNumber, 11);

                //Measured Discharge Temp
                double MeasuredDischargeTemp = objACIkernal.StageInfo(stage.StageNumber, 23);
                //Theoritical Dis. Temp
                // List<int> stageCylinderSet = unit.Stages[stage.StageNumber].CylinderSet;
                List<int> stageCylinderSet = unit.Stages.Where(st => st.Stage == stage.StageNumber).ToList().FirstOrDefault().CylinderSet;
                double sumEstDisTemp = 0;
                foreach (int cylinder in stageCylinderSet)
                {
                    sumEstDisTemp += objACIkernal.HECylinderInfo(cylinder, 4);
                }
                double AvgEstDisTemp = sumEstDisTemp / 2;
                double StgATvsMT = (MeasuredDischargeTemp - AvgEstDisTemp) / (AvgEstDisTemp + 459.67);
                //Ratio Actual Vs Theoritical Temperature
                stage.RatioActualVsTheoriticalTemperature.Value = StgATvsMT;

                //Actual Stage Discharge Pressure
                //  double stgMeasuredDisPressure = Convert.ToDouble(unit.Stages[stage.StageNumber].Pd.Value);//psiG
                var sdp = unit.Stages.Where(st => st.Stage == stage.StageNumber)
                    .ToList()
                    .FirstOrDefault().Pd.Value;//psiG
                double stgMeasuredDisPressure = Convert.ToDouble(sdp);
                //Convert to psiA
                stgMeasuredDisPressure += 14.25;
                //Estimated Stg Discharge Pressure from ACI
                double stgEstimatedDisPressure = objACIkernal.StageInfo(stage.StageNumber, 10);//psiG
                //convert to psiA
                stgEstimatedDisPressure += 14.25;
                double StgAPvsTp = stgMeasuredDisPressure / stgEstimatedDisPressure;
                stage.RatioActualVsTheoriticalPressure.Value = StgAPvsTp;

            }
            return lstStageInfo;

        }
    }
    public class LoadBalance
    {
        public string Stages { get; set; }
        public Tag Ratio { get; set; }

        public static List<LoadBalance> GetLoadBalance(eRCM_KernelControl objACIkernal, List<LoadBalance> stageLoadbalance)
        {
            foreach (var stageLBRatio in stageLoadbalance)
            {
                string[] stLB = stageLBRatio.Stages.Split(':');
                double stgALoad = objACIkernal.StageInfo(Convert.ToInt16(stLB[0]), 11);
                double stgBLoad = objACIkernal.StageInfo(Convert.ToInt16(stLB[1]), 11);
                double stgRatio = stgALoad / stgBLoad;
                stageLBRatio.Ratio.Value = stgRatio;
            }
            return stageLoadbalance;
        }
    }
    public class HeadEndInfo
    {
        public int Cylinder { get; set; }
        public Tag Flow { get; set; }
        public Tag Load { get; set; }
        public Tag DischargeVE { get; set; }
        public Tag SuctionVE { get; set; }

        public static List<HeadEndInfo> GetHeadEndInfo(eRCM_KernelControl objACIkernal, List<HeadEndInfo> outputHeadEndInfo)
        {
            foreach (var cylinder in outputHeadEndInfo)
            {
                cylinder.Load.Value = objACIkernal.HECylinderInfo(cylinder.Cylinder, 12);
                cylinder.Flow.Value = objACIkernal.HECylinderInfo(cylinder.Cylinder, 16);
                cylinder.DischargeVE.Value = objACIkernal.HECylinderInfo(cylinder.Cylinder, 35);
                cylinder.SuctionVE.Value = objACIkernal.HECylinderInfo(cylinder.Cylinder, 36);
            }
            return outputHeadEndInfo;

        }
    }
    public class CrankEndInfo
    {
        public int Cylinder { get; set; }
        public Tag Flow { get; set; }
        public Tag Load { get; set; }
        public Tag DischargeVE { get; set; }
        public Tag SuctionVE { get; set; }
        public Tag EstimatedDischargeTemperature { get; set; }

        public static List<CrankEndInfo> GetHeadEndInfo(eRCM_KernelControl objACIkernal, List<CrankEndInfo> outputCrankEndInfo)
        {
            foreach (var cylinder in outputCrankEndInfo)
            {
                cylinder.Load.Value = objACIkernal.CECylinderInfo(cylinder.Cylinder, 12);
                cylinder.Flow.Value = objACIkernal.CECylinderInfo(cylinder.Cylinder, 16);
                cylinder.DischargeVE.Value = objACIkernal.CECylinderInfo(cylinder.Cylinder, 35);
                cylinder.SuctionVE.Value = objACIkernal.CECylinderInfo(cylinder.Cylinder, 36);
                cylinder.EstimatedDischargeTemperature.Value = objACIkernal.CECylinderInfo(cylinder.Cylinder, 4);
            }
            return outputCrankEndInfo;

        }
    }
    public class CompressionForces
    {
        public int Throw { get; set; }
        public Tag Force { get; set; }
        public static List<CompressionForces> GetCompressionForces(eRCM_KernelControl objACIkernal, List<CompressionForces> outputCompressionForces)
        {
            foreach (var throws in outputCompressionForces)
            {
                throws.Force.Value = objACIkernal.ThrowInfo(throws.Throw, 5);
            }
            return outputCompressionForces;
        }
    }
    public class TensionForces
    {
        public int Throw { get; set; }
        public Tag Force { get; set; }
        public static List<TensionForces> GetTensionForces(eRCM_KernelControl objACIkernal, List<TensionForces> outputTensionForces)
        {
            foreach (var throws in outputTensionForces)
            {
                throws.Force.Value = objACIkernal.ThrowInfo(throws.Throw, 6);
            }
            return outputTensionForces;
        }
    }



}
