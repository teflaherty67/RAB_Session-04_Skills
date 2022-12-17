#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
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
            foreach (CurveElement curCurve in lineList)
            {
                Curve curve = curCurve.GeometryCurve;

                Wall newWall = Wall.Create(doc, curve, newLevel.Id, false);
            }

            t.Commit();
            t.Dispose();

            TaskDialog.Show("Results", "I have " + lineList.Count + " lines.");
            
            return Result.Succeeded;
        }
    }
}
