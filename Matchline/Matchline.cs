using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace Matchline
{
    public class Matchline
    {
        //get the current document and database
        Document acDoc = Application.DocumentManager.MdiActiveDocument;
        Database acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

        [CommandMethod("IML")]
        public void iml()
        {
            Editor ed = acDoc.Editor;

            //prompt select matchline
            PromptEntityOptions entOpt = new PromptEntityOptions("Select Matchline");
            entOpt.SetRejectMessage("Line Selected is not Matchline");
            entOpt.AddAllowedClass(typeof(Line), false);

            //check entity results status
            PromptEntityResult entRst = ed.GetEntity(entOpt);
            if (entRst.Status != PromptStatus.OK)
                return;

            //begin transaction
            Transaction trans = acCurDb.TransactionManager.StartTransaction();

            using (trans)
            {
                //open blockTable for read
                BlockTable blockTable;
                blockTable = trans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                //i think i need to cast the entity to a polyline so i can get the point
                Line matchLine = trans.GetObject(entRst.ObjectId, OpenMode.ForWrite) as Line;

                //get the midpoint of line
                Point3d pts = getMidPoint(matchLine);

                //create text
                DBText mText = new DBText();

                //prompt user for sheet number
                PromptStringOptions strOpt = new PromptStringOptions("Enter Page #")
                {
                    AllowSpaces = false
                };

                string sheetNum = ed.GetString(strOpt).ToString();
                mText.TextString = "MATCHLINE - SEE SHEET " + sheetNum;

                //prompt to select side 
                PromptPointOptions ptOpt = new PromptPointOptions("Select Side ");

                sideSelected(pts, matchLine.EndPoint, matchLine.StartPoint );

                mText.Justify = AttachmentPoint.BottomCenter;

            }
        }

        //calculate the midpoint 
        public Point3d getMidPoint(Line a)
        {


            //DBObjectCollection objcoll = a;
            //Line tempLine = new Line();

            //foreach (Entity ent in objcoll)
            //{
            //    tempLine = ((Line)ent);
            //}

            Point3d point3d = new Point3d();
            Vector3d vector = new Vector3d();
            vector = a.StartPoint.GetVectorTo(a.EndPoint);
            point3d = a.StartPoint + (vector / 2);

            return point3d;
        }

        //create vector with point selected and midpoint of line 
        //crete vector with midpoint and endpoint of line
        public bool sideSelected(Point3d midPoint, Point3d endPoint, Point3d startPoint)
        {
          Vector3d vector1 = midPoint.GetVectorTo(endPoint);
          Vector3d vector2 = midPoint.GetVectorTo(startPoint);

          double ang = vector1.GetAngleTo(vector2);        
          
          
          return true;
        }
    }
}
