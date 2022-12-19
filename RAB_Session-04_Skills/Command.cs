#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RAB_Session_04_Skills
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select elements");

            List<CurveElement> lineList = new List<CurveElement>();
            
            foreach(Element element in pickList)
            {
                if(element is CurveElement)
                {
                    CurveElement curve = (CurveElement)element;

                    if(curve.CurveElementType == CurveElementType.ModelCurve)
                        lineList.Add(curve);
                }
            }

            Transaction t = new Transaction(doc);
            t.Start("Create wall");

            Level newLevel = Level.Create(doc, 15);
            WallType curWT = Utils.GetWallTypeByName(doc, "Storefront");

            MEPSystemType pipeSystemType = Utils.GetMEPSystemTypeByName(doc, "Domestic Hot Water");
            PipeType pipeType = Utils.GetPipeTypeByName(doc, "Default");

            MEPSystemType ductSystemType = Utils.GetMEPSystemTypeByName(doc, "Supply Air");
            DuctType ductType = Utils.GetDuctTypeByName(doc, "Default");
            
            foreach (CurveElement curCurve in lineList)
            {
                GraphicsStyle curGS = curCurve.LineStyle as GraphicsStyle;
                Debug.Print(curGS.Name);

                Curve curve = curCurve.GeometryCurve;
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);

                // Wall newWall = Wall.Create(doc, curve, curWT.Id, newLevel.Id, 20, 0, false, false);

                // Pipe newPipe = Pipe.Create(doc, pipeSystemType.Id, pipeType.Id, newLevel.Id, startPoint, endPoint);

                Duct newDuct = Duct.Create(doc, ductSystemType.Id, ductType.Id, newLevel.Id, startPoint, endPoint);

            }

            t.Commit();
            t.Dispose();

            TaskDialog.Show("Results", "I have " + lineList.Count + " lines.");
            
            return Result.Succeeded;
        }

        
    }
}
