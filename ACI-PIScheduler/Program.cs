using Newtonsoft.Json;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.PI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eRCM_Kernel;
using OSIsoft.AF.Data;
using System.Threading;
using System.Reflection;

namespace ACI_PIScheduler
{
    class Program
    {
        static string aciLicense = ConfigurationManager.AppSettings["aciLicense"];
        static string piServerName = ConfigurationManager.AppSettings["piServerName"];
        static string aciInputFile = ConfigurationManager.AppSettings["inputACI"];
        static string aciOutputFile = ConfigurationManager.AppSettings["outputACI"];
        static string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static PIServers piServers = new PIServers();
        static PIServer piServer = piServers[piServerName];

        public static eRCM_KernelControl objERCMKernal;
        static void Main(string[] args)
        {

            try
            {
                AFData.BufferOption = AFBufferOption.Buffer;
                aciInputFile = Path.Combine(executableLocation, aciInputFile);
                aciOutputFile = Path.Combine(executableLocation, aciOutputFile);

                if (!CheckACIKernalLicence())
                {
                    Trace.WriteLine(DateTime.Now);
                    Trace.WriteLine("ACI licence expired");
                    return;
                }

                //Read input tags from JSON
                var lstServices = ReadInputTags();
                foreach (var service in lstServices.ServiceList)
                {
                    ProcessViewerFiles(service);
                }
                Console.WriteLine("Running-> " + DateTime.Now);
            }
            catch (Exception ex)
            {

                Trace.WriteLine(ex.Message);
            }
        }

        public static bool CheckACIKernalLicence()
        {
            eRCM_Kernel.eRCM_KernelControl.SetLicense(aciLicense);
            objERCMKernal = new eRCM_Kernel.eRCM_KernelControl();
            if (objERCMKernal.IsLicenseValid)
                return true;
            return false;
        }

        public static Services ReadInputTags()
        {
            Services objServiceList = JsonConvert.DeserializeObject<Services>(File.ReadAllText(aciInputFile));
            return objServiceList;
        }

        public static void ProcessViewerFiles(Service service)
        {
            //  string viewerFilePath = String.Format("ViewerFiles/{0}.rvf", service.ViewerFileName);//1
            string viewerFilePath = service.ViewerFileName;
            viewerFilePath = Path.Combine(executableLocation, viewerFilePath);
            Trace.WriteLine(service.ServiceName + " Unit->" + service.UnitName + " " + DateTime.Now);
            Console.WriteLine(service.UnitName + " " + DateTime.Now);

            service = GetInputTagValueFromPi(service);
            GetACIOutput(service, viewerFilePath);

            //if (service.ServiceName == "Thunder")
            //{
            //    service = GetInputTagValueFromPi(service);
            //    GetACIOutput(service, viewerFilePath);
            //}

            //if (service.ServiceName == "Lightning" && service.UnitName == "Unit 2")
            //{
            //    service = GetInputTagValueFromPi(service);
            //    GetACIOutput(service, viewerFilePath);
            //}


        }

        public static string[] GetTagArray(Service unit)
        {

            List<string> unitTagList = new List<string>
            {
                unit.EngineSpeed.Name,
                unit.DischargePressure.Name,
                unit.StageSuctionPressure.Ps1.Name,
                unit.StageSuctionTemperature.Ts1.Name,
               // unit.StageSuctionTemperature.Ts2.Name
            };
            if (unit.StageSuctionTemperature.Ts2 != null)
                unitTagList.Add(unit.StageSuctionTemperature.Ts2.Name);
            if (unit.StageSuctionTemperature.Ts3 != null)
                unitTagList.Add(unit.StageSuctionTemperature.Ts3.Name);
            if (unit.StageSuctionTemperature.Ts4 != null)
                unitTagList.Add(unit.StageSuctionTemperature.Ts4.Name);
            if (unit.StageSuctionTemperature.Ts5 != null)
                unitTagList.Add(unit.StageSuctionTemperature.Ts5.Name);
            if (unit.StageSuctionTemperature.Ts6 != null)
                unitTagList.Add(unit.StageSuctionTemperature.Ts6.Name);
            foreach (var stage in unit.Stages)
            {
                unitTagList.Add(stage.Pd.Name);
            }
            return unitTagList.ToArray();

        }

        public static Service GetInputTagValueFromPi(Service unit)
        {
            var unitTagList = GetTagArray(unit);
            AFListResults<PIPoint, AFValue> PiDataValues = ReadValuesFromPi(unitTagList);
            foreach (var piValue in PiDataValues)
            {
                if (unit.EngineSpeed.Name == piValue.PIPoint.Name)
                {
                    unit.EngineSpeed.Value = piValue.Value;
                }
                if (unit.DischargePressure.Name == piValue.PIPoint.Name)
                {
                    unit.DischargePressure.Value = piValue.Value;
                }

                if (unit.StageSuctionPressure.Ps1.Name == piValue.PIPoint.Name)
                {
                    unit.StageSuctionPressure.Ps1.Value = piValue.Value;
                }
                //if (unit.StageSuctionPressure.Ps2.Name == piValue.PIPoint.Name)
                //{
                //    unit.StageSuctionPressure.Ps2.Value = piValue.Value;
                //}
                if (unit.StageSuctionTemperature.Ts1.Name == piValue.PIPoint.Name)
                {
                    unit.StageSuctionTemperature.Ts1.Value = piValue.Value;
                }
                //if (unit.StageSuctionTemperature.Ts2.Name == piValue.PIPoint.Name
                //{
                //    unit.StageSuctionTemperature.Ts2.Value = piValue.Value;
                //}
                if (unit.StageSuctionTemperature.Ts2 != null)
                {
                    if (unit.StageSuctionTemperature.Ts2.Name == piValue.PIPoint.Name)
                    {
                        unit.StageSuctionTemperature.Ts2.Value = piValue.Value;
                    }
                }

                if (unit.StageSuctionTemperature.Ts3 != null)
                {
                    if (unit.StageSuctionTemperature.Ts3.Name == piValue.PIPoint.Name)
                    {
                        unit.StageSuctionTemperature.Ts3.Value = piValue.Value;
                    }
                }

                if (unit.StageSuctionTemperature.Ts4 != null)
                {
                    if (unit.StageSuctionTemperature.Ts4.Name == piValue.PIPoint.Name)
                    {
                        unit.StageSuctionTemperature.Ts4.Value = piValue.Value;
                    }
                }
                if (unit.StageSuctionTemperature.Ts5 != null)
                {
                    if (unit.StageSuctionTemperature.Ts5.Name == piValue.PIPoint.Name)
                    {
                        unit.StageSuctionTemperature.Ts5.Value = piValue.Value;
                    }
                }
                if (unit.StageSuctionTemperature.Ts6 != null)
                {
                    if (unit.StageSuctionTemperature.Ts6.Name == piValue.PIPoint.Name)
                    {
                        unit.StageSuctionTemperature.Ts6.Value = piValue.Value;
                    }
                }
                foreach (var stage in unit.Stages)
                {
                    if (stage.Pd.Name == piValue.PIPoint.Name)
                    {
                        stage.Pd.Value = piValue.Value;
                    }
                }

            }

            return unit;


        }

        public static void GetACIOutput(Service unit, string ViewerFilePath)
        {
            PISystems piSystems = new PISystems();
            PISystem piSystem;
            AFDatabase database;

            string error = "";
            string error1 = "";

            Outputs objOutputUnit = null;
            CurrentOutputAtGivenLoadStep resCurrentOutputs = null;
            IdealOutputAtGivenTorque resIdealOtpsAt90T = null, resIdealOtpsAt95T = null, resIdealOtpsAt100T = null;

            IList<PIPoint> lstPiPointForUnit = null;
            AFNamedCollectionList<AFElement> AllElements = new AFNamedCollectionList<AFElement>();
            AFValue floatingValue = null;

            AFElement Model = new AFElement();
            AFAttribute ErrorArrayCode = null;

            try
            {
                piSystem = piSystems["WINOSIT3D01"];
                database = piSystem.Databases["DCPDev"];

                if (File.Exists(ViewerFilePath))
                {
                    // Trace.Write(unit.UnitName + "   :"); Trace.WriteLine(ViewerFilePath);

                    var objERCM = objERCMKernal;
                    objERCM.Reset();
                    objERCM.eRCMViewerFilename = ViewerFilePath;
                    AciModelInputs objAciInput = new AciModelInputs(unit);
                    if (objERCM.SuccessfulFileLoad)
                    {
                        if (!string.IsNullOrEmpty(objAciInput.ErrorMsg))
                        {
                            Trace.TraceError("Valid Input data not available for " + unit.UnitName + " at" + DateTime.Now);
                            return;
                        }

                        objOutputUnit = GetACIOutputObjectForUnit(unit.UnitName, unit.ServiceName);

                        if (objAciInput.Rpm < 100)
                        {
                            error1 = " RPM is less than 100. Model wouold not run for " + unit.ServiceName + " " + unit.UnitName;
                            Console.WriteLine(error1);

                            AllElements = AFElement.FindElements(database, null, null, AFSearchField.Name, true, AFSortField.Name, AFSortOrder.Ascending, 1000000000);


                            foreach (AFElement ele in AllElements)
                            {
                                if (ele.Name.Equals(unit.UnitName) && ((ele.Parent != null && ele.Parent.Name.Equals(unit.ServiceName)) || (ele.Parent.Parent != null && ele.Parent.Parent.Name.Equals(unit.ServiceName))))
                                {
                                    Model = ele.Elements["Model"];
                                    ErrorArrayCode = Model.Attributes["ACI Error Array Code"];

                                    floatingValue = new AFValue(error1);
                                    ErrorArrayCode.Data.UpdateValue(floatingValue, AFUpdateOption.NoReplace, AFBufferOption.DoNotBuffer);
                                }
                            }

                            return;
                        }

                        objERCM.CurrentLoadStep = unit.LoadStep;
                        resCurrentOutputs = GetCurrentOutputAtGivenLoadStep(unit.LoadStep, objAciInput, objERCM);
                        resIdealOtpsAt90T = GetIdealOutputAtGivenTorque(unit.LoadStep, 90, objAciInput, objERCM);
                        resIdealOtpsAt95T = GetIdealOutputAtGivenTorque(unit.LoadStep, 95, objAciInput, objERCM);
                        resIdealOtpsAt100T = GetIdealOutputAtGivenTorque(unit.LoadStep, 100, objAciInput, objERCM);

                        objERCM.TorqueLimit = 100;
                        objERCM.CurrentLoadStep = unit.LoadStep;
                        objERCM.ChangeOpCondition(objAciInput.PsG, objAciInput.PdG, objAciInput.Rpm,
                            objAciInput.Ts1, objAciInput.Ts2, objAciInput.Ts3, objAciInput.Ts4, objAciInput.Ts5, objAciInput.Ts6);

                        string ACIInputValues = "PsG:" + objAciInput.PsG + " PdG:" + objAciInput.PdG + " Rpm:" + objAciInput.Rpm + " Ts1:" +
                            objAciInput.Ts1 + " Ts2:" + objAciInput.Ts2 + " Ts3:" + objAciInput.Ts3 + " Ts4:" + objAciInput.Ts4 + " Ts5:" + objAciInput.Ts5 + " Ts6:" + objAciInput.Ts6;
                        Trace.TraceInformation(ACIInputValues);
                        //Max Allowed Dischage Pressure
                        var MaxAllowedPd = objERCM.CalcMaxAllowedPd(objAciInput.PsG, objAciInput.Rpm, objAciInput.Ts1, objAciInput.Ts2, objAciInput.Ts3, objAciInput.Ts4, objAciInput.Ts5, objAciInput.Ts6, unit.LoadStep);
                        objERCM.ErrorArray(objERCM.CurrentLoadStep);
                        error = objERCM.FullEnglishErrors(objERCM.ErrorArray(objERCM.CurrentLoadStep), 0);

                        if (error != string.Empty)
                        {
                            Trace.TraceInformation(ACIInputValues);
                            Trace.TraceError(error);
                        }
                        error = error + error1;

                        string CurrentTorq = objERCM.CurrentTorque.ToString();
                        //Output Result
                        //objOutputUnit = GetACIOutputObjectForUnit(unit.UnitName, unit.ServiceName);//at here Error Occurs!!!
                        //Assign Value to tags
                        lstPiPointForUnit = GetOutputTagList(objOutputUnit);
                        //Update in PI Server
                        // var result = SaveOutputDataInPi(objUnit, lstPiPointForUnit, objERCM);
                        objOutputUnit.MaxAllowedDischargePressure.Value = MaxAllowedPd;

                        //Current Operating Outputs
                        objOutputUnit.LoadStep.Current.Value = resCurrentOutputs.CurrentLoadStep;
                        objOutputUnit.LoadStep.CurrentDetails.Value = resCurrentOutputs.CurrentLoadStepDetail;
                        objOutputUnit.LoadPrediction.Current.Value = resCurrentOutputs.CurrentLoad;
                        objOutputUnit.FlowPrediction.Current.Value = resCurrentOutputs.CurrentFlow;
                        objOutputUnit.FuelRate.Current.Value = resCurrentOutputs.CurrentFuelRate;

                        //Ideal Outputs at 90% Torque
                        objOutputUnit.IdealFlow90T.Value = resIdealOtpsAt90T.IdealFlowAtGivenTorque;
                        objOutputUnit.IdealFuelRate90T.Value = resIdealOtpsAt90T.IdealFuelRateAtGivenTorque;
                        objOutputUnit.IdealLoad90T.Value = resIdealOtpsAt90T.IdealLoadAtGivenTorque;
                        objOutputUnit.IdealLoadStep90T.Value = resIdealOtpsAt90T.IdealLoadStepAtGivenTorque;
                        objOutputUnit.IdealLoadStepDetail90T.Value = resIdealOtpsAt90T.IdealLoadStepDetailAtGivenTorque;
                        //Ideal Outputs at 95% Torque
                        objOutputUnit.IdealFlow95T.Value = resIdealOtpsAt95T.IdealFlowAtGivenTorque;
                        objOutputUnit.IdealFuelRate95T.Value = resIdealOtpsAt95T.IdealFuelRateAtGivenTorque;
                        objOutputUnit.IdealLoad95T.Value = resIdealOtpsAt95T.IdealLoadAtGivenTorque;
                        objOutputUnit.IdealLoadStep95T.Value = resIdealOtpsAt95T.IdealLoadStepAtGivenTorque;
                        objOutputUnit.IdealLoadStepDetail95T.Value = resIdealOtpsAt95T.IdealLoadStepDetailAtGivenTorque;
                        //Ideal Outputs at 100% Torque    
                        objOutputUnit.LoadStep.Ideal.Value = resIdealOtpsAt100T.IdealLoadStepAtGivenTorque;
                        objOutputUnit.LoadStep.IdealDetails.Value = resIdealOtpsAt100T.IdealLoadStepDetailAtGivenTorque;
                        objOutputUnit.LoadPrediction.Ideal.Value = resIdealOtpsAt100T.IdealLoadAtGivenTorque;
                        objOutputUnit.FlowPrediction.Ideal.Value = resIdealOtpsAt100T.IdealFlowAtGivenTorque;
                        objOutputUnit.FuelRate.Ideal.Value = resIdealOtpsAt100T.IdealFuelRateAtGivenTorque;

                        if (resCurrentOutputs.CurrentFlow < resIdealOtpsAt100T.IdealFlowAtGivenTorque || resCurrentOutputs.CurrentLoad < resIdealOtpsAt100T.IdealLoadAtGivenTorque)
                        {
                            Console.WriteLine("Current load & Flow lower than ideal.");
                            string message = string.Format("Service Name:{0}, Unit Name:{1},Current Load:{2},Ideal Load:{3}, Current Flow:{4}, Ideal Flow: {5}",
                                unit.ServiceName, unit.UnitName, resCurrentOutputs.CurrentLoad, resIdealOtpsAt100T.IdealLoadAtGivenTorque, resCurrentOutputs.CurrentFlow, resIdealOtpsAt100T.IdealFlowAtGivenTorque);
                            Trace.WriteLine(message);
                            //Ideal Outputs at 90% Torque
                            objOutputUnit.IdealFlow90T.Value = "No Data";
                            objOutputUnit.IdealFuelRate90T.Value = "No Data";
                            objOutputUnit.IdealLoad90T.Value = "No Data";
                            objOutputUnit.IdealLoadStep90T.Value = "No Data";
                            objOutputUnit.IdealLoadStepDetail90T.Value = "No Data";
                            //Ideal Outputs at 95% Torque
                            objOutputUnit.IdealFlow95T.Value = "No Data";
                            objOutputUnit.IdealFuelRate95T.Value = "No Data";
                            objOutputUnit.IdealLoad95T.Value = "No Data";
                            objOutputUnit.IdealLoadStep95T.Value = "No Data";
                            objOutputUnit.IdealLoadStepDetail95T.Value = "No Data";
                            objOutputUnit.LoadStep.Ideal.Value = "No Data";
                            objOutputUnit.LoadStep.IdealDetails.Value = "No Data";
                            objOutputUnit.LoadPrediction.Ideal.Value = "No Data";
                            objOutputUnit.FlowPrediction.Ideal.Value = "No Data";
                            objOutputUnit.FuelRate.Ideal.Value = "No Data";
                        }

                        objOutputUnit.ErrorCode.Value = error;
                        SaveOutputDataInPi(objOutputUnit, lstPiPointForUnit, objERCM, unit);

                    }
                    else
                    {
                        Trace.WriteLine("Failed to load in Viewer file!");
                    }
                }
                else
                {
                    Trace.WriteLine("File doesn't exist at " + ViewerFilePath);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
        }

        public static IdealOutputAtGivenTorque GetIdealOutputAtGivenTorque(int CurrentLoadStep, double torque, AciModelInputs objAciInput, eRCM_KernelControl objACIkernal)
        {
            IdealOutputAtGivenTorque resIdealOutput = new IdealOutputAtGivenTorque();
            objACIkernal.TorqueLimit = torque;
            objACIkernal.CurrentLoadStep = CurrentLoadStep;
            objACIkernal.ChangeOpCondition(objAciInput.PsG, objAciInput.PdG, objAciInput.Rpm,
                       objAciInput.Ts1, objAciInput.Ts2, objAciInput.Ts3, objAciInput.Ts4, objAciInput.Ts5, objAciInput.Ts6);
            //Ideal Load Step
            resIdealOutput.IdealLoadStepAtGivenTorque = objACIkernal.FindOptimalLoadStep(1, 0, 0);
            resIdealOutput.IdealLoadStepDetailAtGivenTorque = objACIkernal.LoadStepName(objACIkernal.FindOptimalLoadStep(1, 0, 0));
            resIdealOutput.IdealLoadAtGivenTorque = objACIkernal.LoadArray(objACIkernal.FindOptimalLoadStep(1, 0, 0));
            resIdealOutput.IdealFlowAtGivenTorque = objACIkernal.FlowArray(objACIkernal.FindOptimalLoadStep(1, 0, 0));
            resIdealOutput.IdealFuelRateAtGivenTorque = objACIkernal.EngineDataFuelRate();
            return resIdealOutput;
        }

        public static CurrentOutputAtGivenLoadStep GetCurrentOutputAtGivenLoadStep(int CurrentLoadStep, AciModelInputs objAciInput, eRCM_KernelControl objACIkernal)
        {
            CurrentOutputAtGivenLoadStep resCurrentOutput = new CurrentOutputAtGivenLoadStep();
            objACIkernal.CurrentLoadStep = CurrentLoadStep;
            objACIkernal.ChangeOpCondition(objAciInput.PsG, objAciInput.PdG, objAciInput.Rpm,
                       objAciInput.Ts1, objAciInput.Ts2, objAciInput.Ts3, objAciInput.Ts4, objAciInput.Ts5, objAciInput.Ts6);
            //Ideal Load Step
            resCurrentOutput.CurrentLoadStep = CurrentLoadStep;
            resCurrentOutput.CurrentLoadStepDetail = objACIkernal.LoadStepName(CurrentLoadStep);
            resCurrentOutput.CurrentLoad = objACIkernal.LoadArray(CurrentLoadStep);
            resCurrentOutput.CurrentFlow = objACIkernal.FlowArray(CurrentLoadStep);
            resCurrentOutput.CurrentFuelRate = objACIkernal.EngineDataFuelRate();
            return resCurrentOutput;

        }

        public static IList<PIPoint> GetOutputTagList(Outputs outputUnitObject)
        {
            var unit = outputUnitObject;
            List<string> TagArray = new List<string>
            {
                //Load Step tags
                unit.LoadStep.Current.Name,
                unit.LoadStep.CurrentDetails.Name,
                unit.LoadStep.Ideal.Name,
                unit.LoadStep.IdealDetails.Name,
                unit.LoadStep.NextUp.Name,
                unit.LoadStep.NextDown.Name,

                //Load Prediction
                unit.LoadPrediction.Current.Name,
                unit.LoadPrediction.Ideal.Name,

                //Flow Prediction
                unit.FlowPrediction.Current.Name,
                unit.FlowPrediction.Ideal.Name,

                //Fuel Rates
                unit.FuelRate.Current.Name,
                unit.FuelRate.Ideal.Name,
                //Mechanical Efficiency
                unit.MechanicalEfficiency.Name,
                //Current Torque
                unit.CurrentTorque.Name,
                //Max Load
                unit.MaxLoad.Name,
                //ErrorCode
                unit.ErrorCode.Name,
                //Rated Load
                unit.RatedLoad.Name,
                 //Rated Spped
                 unit.RatedSpeed.Name,
                //Min Load/Flow Change Allowed
                unit.MinChangeAllowed.Name,
                //Max Load Change Allowed
                unit.MaxChangeAllowed.Name,
                //Min Speed 
                unit.MinSpeed.Name,
                //Max Speed
                unit.MaxSpeed.Name,
                //Max Allowed Discharge Pressure
                unit.MaxAllowedDischargePressure.Name,
                unit.IdealFlow90T.Name,
                unit.IdealFuelRate90T.Name,
                unit.IdealLoad90T.Name,
                unit.IdealLoadStep90T.Name,
                unit.IdealLoadStepDetail90T.Name,
                unit.IdealFlow95T.Name,
                unit.IdealFuelRate95T.Name,
                unit.IdealLoad95T.Name,
                unit.IdealLoadStep95T.Name,
                unit.IdealLoadStepDetail95T.Name

            };
            //StageInfo
            foreach (var stage in unit.StageInfo)
            {
                int stageNumber = stage.StageNumber;
                TagArray.Add(stage.CompressionRatio.Name);
                TagArray.Add(stage.DischargePressureAtFlange.Name);
                TagArray.Add(stage.DischargePressureAtGadge.Name);
                TagArray.Add(stage.DischargeTemperature.Name);
                TagArray.Add(stage.Load.Name);
                TagArray.Add(stage.RatioActualVsTheoriticalPressure.Name);
                TagArray.Add(stage.RatioActualVsTheoriticalTemperature.Name);
            }
            //Load Balance for stages
            foreach (var stage in unit.LoadBalance)
            {
                string stagesRatio = stage.Stages;
                TagArray.Add(stage.Ratio.Name);
            }
            //Head End Info
            foreach (var cylinder in unit.HeadEndInfo)
            {
                int cylNumber = cylinder.Cylinder;
                TagArray.Add(cylinder.Flow.Name);
                TagArray.Add(cylinder.Load.Name);
                TagArray.Add(cylinder.DischargeVE.Name);
                TagArray.Add(cylinder.SuctionVE.Name);
            }
            //Crank End Info
            foreach (var cylinder in unit.CrankEndInfo)
            {
                int cylNumber = cylinder.Cylinder;
                TagArray.Add(cylinder.Flow.Name);
                TagArray.Add(cylinder.Load.Name);
                TagArray.Add(cylinder.DischargeVE.Name);
                TagArray.Add(cylinder.SuctionVE.Name);
                TagArray.Add(cylinder.EstimatedDischargeTemperature.Name);
            }
            //Compression Forces
            foreach (var Throw in unit.CompressionForces)
            {
                int thowNumber = Throw.Throw;
                TagArray.Add(Throw.Force.Name);
            }
            //TensionForces
            foreach (var Throw in unit.TensionForces)
            {
                int thowNumber = Throw.Throw;
                TagArray.Add(Throw.Force.Name);
            }

            IList<PIPoint> points = PIPoint.FindPIPoints(piServer, TagArray.ToArray());
            return points;

        }

        public static Outputs GetACIOutputObjectForUnit(string UnitName, string ServiceName)
        {
            OutputList objoutput = JsonConvert.DeserializeObject<OutputList>(File.ReadAllText(aciOutputFile));
            var unit = objoutput.Outputs.Where(o => o.UnitName == UnitName && o.ServiceName == ServiceName).FirstOrDefault();
            return unit;
        }
        public static void SaveOutputDataInPi(Outputs UnitOutputTags, IList<PIPoint> points, eRCM_KernelControl objACIkernal, Service unit)
        {
            IList<AFValue> valuesToWrite = new List<AFValue>();
            List<Tag> lstTagResult = new List<Tag>();
            var LoadArray = objACIkernal.LoadArray(unit.LoadStep);
            //Load Step
            object IdealLS = UnitOutputTags.LoadStep.Ideal.Value;
            object IdealLSDetails = UnitOutputTags.LoadStep.IdealDetails.Value;
            var loadStep = LoadSteps.GetLoadSteps(objACIkernal, UnitOutputTags.LoadStep);
            //lstTagResult.Add(loadStep.Current);
            //lstTagResult.Add(loadStep.CurrentDetails);
            //lstTagResult.Add(loadStep.Ideal);
            //lstTagResult.Add(loadStep.IdealDetails);
            lstTagResult.Add(loadStep.NextDown);
            lstTagResult.Add(loadStep.NextUp);
            //Load Prediction
            //var loadPredictionResult = LoadPrediction.GetLoadPrediction(objACIkernal, UnitOutputTags.LoadPrediction);
            //lstTagResult.Add(loadPredictionResult.Current);
            //lstTagResult.Add(loadPredictionResult.Ideal);
            //Flow Prediction
            //var flowPredictionResult = FlowPrediction.GetFlowPrediction(objACIkernal, UnitOutputTags.FlowPrediction);
            //lstTagResult.Add(flowPredictionResult.Current);
            //lstTagResult.Add(flowPredictionResult.Ideal);
            //Meachinical Efficiency
            UnitOutputTags.MechanicalEfficiency.Value = objACIkernal.MechanicalEfficiency;
            lstTagResult.Add(UnitOutputTags.MechanicalEfficiency);
            //Current Torque
            UnitOutputTags.CurrentTorque.Value = objACIkernal.CurrentTorque;
            lstTagResult.Add(UnitOutputTags.CurrentTorque);
            //Max Load
            UnitOutputTags.MaxLoad.Value = objACIkernal.MaxAllowedLoad;
            lstTagResult.Add(UnitOutputTags.MaxLoad);
            //ErroCode objERCM.FullEnglishErrors(objERCM.ErrorArray(objERCM.CurrentLoadStep), 0)
            //UnitOutputTags.ErrorCode.Value = objACIkernal.FullEnglishErrors(objACIkernal.ErrorArray(objACIkernal.CurrentLoadStep), 0);
            //UnitOutputTags.ErrorCode.Value = objACIkernal.ErrorArray(objACIkernal.CurrentLoadStep);
            //lstTagResult.Add(UnitOutputTags.ErrorCode);
            //Rated Load            
            UnitOutputTags.RatedLoad.Value = objACIkernal.RatedLoad;
            lstTagResult.Add(UnitOutputTags.RatedLoad);
            //Rated Speed
            UnitOutputTags.RatedSpeed.Value = objACIkernal.RatedSpeed;
            lstTagResult.Add(UnitOutputTags.RatedSpeed);
            //Min Load/Flow Change Allowed
            UnitOutputTags.MinChangeAllowed.Value = objACIkernal.MinLoadFlowChangeAllowed;
            lstTagResult.Add(UnitOutputTags.MinChangeAllowed);
            //Max Load Change Allowed
            UnitOutputTags.MaxChangeAllowed.Value = objACIkernal.MaxLoadChangeAllowed;
            lstTagResult.Add(UnitOutputTags.MaxChangeAllowed);
            //Min Speed
            UnitOutputTags.MinSpeed.Value = objACIkernal.MinSpeedCurrentLS();
            lstTagResult.Add(UnitOutputTags.MinSpeed);
            //Max Speed
            UnitOutputTags.MaxSpeed.Value = objACIkernal.MaxSpeedCurrentLS();
            lstTagResult.Add(UnitOutputTags.MaxSpeed);
            //Stage Info
            var stageInfoResult = StageInfo.GetStageInfoList(objACIkernal, UnitOutputTags.StageInfo, unit);
            foreach (var stage in stageInfoResult)
            {
                // Trace.WriteLine("StageNumber:" + stage.StageNumber);
                lstTagResult.Add(stage.CompressionRatio);
                lstTagResult.Add(stage.DischargePressureAtFlange);
                lstTagResult.Add(stage.DischargePressureAtGadge);
                lstTagResult.Add(stage.DischargeTemperature);
                lstTagResult.Add(stage.Load);
                lstTagResult.Add(stage.RatioActualVsTheoriticalPressure);
                lstTagResult.Add(stage.RatioActualVsTheoriticalTemperature);
            }
            //Load Balance
            var lbResult = LoadBalance.GetLoadBalance(objACIkernal, UnitOutputTags.LoadBalance);
            foreach (var stage in lbResult)
            {
                // Trace.WriteLine("StageNumber:" + stage.Stages);
                lstTagResult.Add(stage.Ratio);
            }
            //Head End Info
            var headEndResult = HeadEndInfo.GetHeadEndInfo(objACIkernal, UnitOutputTags.HeadEndInfo);
            foreach (var headEndCylinder in headEndResult)
            {
                lstTagResult.Add(headEndCylinder.DischargeVE);
                lstTagResult.Add(headEndCylinder.Flow);
                lstTagResult.Add(headEndCylinder.Load);
                lstTagResult.Add(headEndCylinder.SuctionVE);
            }
            //Crank End
            var crankEndResult = CrankEndInfo.GetHeadEndInfo(objACIkernal, UnitOutputTags.CrankEndInfo);
            foreach (var crankEndCylinder in crankEndResult)
            {
                lstTagResult.Add(crankEndCylinder.DischargeVE);
                lstTagResult.Add(crankEndCylinder.Flow);
                lstTagResult.Add(crankEndCylinder.Load);
                lstTagResult.Add(crankEndCylinder.SuctionVE);
                lstTagResult.Add(crankEndCylinder.EstimatedDischargeTemperature);
            }
            //Compression Forces
            var compForcesResults = CompressionForces.GetCompressionForces(objACIkernal, UnitOutputTags.CompressionForces);
            foreach (var throws in compForcesResults)
            {
                lstTagResult.Add(throws.Force);
            }
            //Tension Forces
            var tensForcesResults = TensionForces.GetTensionForces(objACIkernal, UnitOutputTags.TensionForces);
            foreach (var throws in tensForcesResults)
            {
                lstTagResult.Add(throws.Force);
            }
            //Fuel Rate
            //var fuelRatesResult = FuelRates.GetFuelRates(objACIkernal, UnitOutputTags.FuelRate, unit);
            //lstTagResult.Add(fuelRatesResult.Current);
            //lstTagResult.Add(fuelRatesResult.Ideal);
            lstTagResult.Add(UnitOutputTags.MaxAllowedDischargePressure);
            lstTagResult.Add(UnitOutputTags.IdealFlow90T);
            lstTagResult.Add(UnitOutputTags.IdealFuelRate90T);
            lstTagResult.Add(UnitOutputTags.IdealLoad90T);
            lstTagResult.Add(UnitOutputTags.IdealLoadStep90T);
            lstTagResult.Add(UnitOutputTags.IdealLoadStepDetail90T);
            lstTagResult.Add(UnitOutputTags.IdealFlow95T);
            lstTagResult.Add(UnitOutputTags.IdealFuelRate95T);
            lstTagResult.Add(UnitOutputTags.IdealLoad95T);
            lstTagResult.Add(UnitOutputTags.IdealLoadStep95T);
            lstTagResult.Add(UnitOutputTags.IdealLoadStepDetail95T);
            UnitOutputTags.LoadStep.Ideal.Value = IdealLS;
            lstTagResult.Add(UnitOutputTags.LoadStep.Ideal);
            UnitOutputTags.LoadStep.IdealDetails.Value = IdealLSDetails;
            lstTagResult.Add(UnitOutputTags.LoadStep.IdealDetails);
            lstTagResult.Add(UnitOutputTags.LoadPrediction.Ideal);
            lstTagResult.Add(UnitOutputTags.FlowPrediction.Ideal);
            lstTagResult.Add(UnitOutputTags.FuelRate.Ideal);
            lstTagResult.Add(UnitOutputTags.LoadStep.Current);
            lstTagResult.Add(UnitOutputTags.LoadStep.CurrentDetails);
            lstTagResult.Add(UnitOutputTags.LoadPrediction.Current);
            lstTagResult.Add(UnitOutputTags.FlowPrediction.Current);
            lstTagResult.Add(UnitOutputTags.FuelRate.Current);
            lstTagResult.Add(UnitOutputTags.ErrorCode);

            foreach (var pipoint in points)
            {
                var tagValue = lstTagResult.Where(x => x.Name == pipoint.Name).Select(x => x.Value).FirstOrDefault();
                AFValue afValueFloat = new AFValue(tagValue, DateTime.Now)
                {
                    PIPoint = pipoint
                };
                valuesToWrite.Add(afValueFloat);
            }

            piServer.UpdateValues(valuesToWrite, AFUpdateOption.InsertNoCompression, AFBufferOption.BufferIfPossible);
            Console.WriteLine("Pi Values Added for " + unit.UnitName + " at" + DateTime.Now + " Successfully");
        }
        public static AFListResults<PIPoint, AFValue> ReadValuesFromPi(string[] tagnam)
        {
            IList<PIPoint> points = PIPoint.FindPIPoints(piServer, tagnam);

            // Create an PIPointList object in order to make the bulk call later.
            PIPointList pointList = new PIPointList(points);

            if (pointList == null) return null;

            // MAKE A BULK CALL TO THE PI DATA ARCHIVE
            return pointList.CurrentValue(); // Requires AF SDK 2.7+           

        }

    }
}
