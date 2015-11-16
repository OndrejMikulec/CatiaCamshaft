/*
 * Created by SharpDevelop.
 * User: cd
 * Date: 14. 6. 2015
 * Time: 16:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using INFITF;
using MECMOD;
using PARTITF;

namespace CatiaCamshaft
{
	class Program
	{
			static int iNumberOfCylinders = 4;
			static double iCamThickness      = 15;
			static double iCircle1Rad        = 25;
			static double iCircle2Rad        = 15;
			static double iCircleDist        = 35;
			static double iCenterY           = 0;
			static double iCenterX           = 0;
			static double iCylinderSpacing   = 100;
			static double iPinDiam           = 15;
			static double iPinLength         = 20;
			static double iBearingDiam       = 32;
			static double iBearingLength     = iCylinderSpacing - iPinLength - 2*iCamThickness;
			static double dPi                = 3.14159265358979323846;
			static double iCurrentLevel      	  = 0;
			
			static Sketch oCurrentSketch;
			static Line2D oCurrentLine1;
			static Line2D oCurrentLine2;
			static Circle2D oCurrentCircle1;
			static Circle2D oCurrentCircle2;
			static Reference oPlaneYZ;
			
			static PartDocument oPartDocument;
			static Part oPart;
			static Body oPartBody;
		
		public static void Main(string[] args)
		{
			Application CATIA = (Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Catia.Application");
			oPartDocument = (PartDocument)CATIA.Documents.Add ( "Part" );
			oPart = oPartDocument.Part;
			oPartBody = oPart.MainBody;
			oPlaneYZ = oPart.CreateReferenceFromGeometry( oPart.OriginElements.PlaneYZ );

			//CATIA.ActiveWindow.ActiveViewer.RenderingMode = 1;
			//msgbox "Create Five Bearings"
			CreatePatternBearing();
			oPart.Update();
			CATIA.ActiveWindow.ActiveViewer.Reframe();
			//msgbox "Create First Cam Set"
			CreateCamSet (0);
			oPart.Update();
			CATIA.ActiveWindow.ActiveViewer.Reframe();
			//msgbox "Create Second Cam Set"
			CreateCamSet (90);
			oPart.Update();
			CATIA.ActiveWindow.ActiveViewer.Reframe();
			//msgbox "Create Third Cam Set"
			CreateCamSet (180);
			oPart.Update();
			CATIA.ActiveWindow.ActiveViewer.Reframe();
			//msgbox "Create Fourth Cam Set"
			CreateCamSet (270);
			oPart.Update();
			CATIA.ActiveWindow.ActiveViewer.Reframe();
			//msgbox "Create Driving Wheel"
			CreateCylinder (iPinLength/2, iBearingDiam );
			oPart.Update();
			CATIA.ActiveWindow.ActiveViewer.Reframe();
			//msgbox "This is the macro end"

		}

		public static void CreatePatternBearing()
		{
			oCurrentSketch = oPartBody.Sketches.Add ( oPlaneYZ );
			Factory2D oFactory2D = oCurrentSketch.OpenEdition();
			oCurrentCircle1 = oFactory2D.CreateClosedCircle ( iCenterX, iCenterY, iBearingDiam/2 );
			oCurrentSketch.CloseEdition();
			ShapeFactory oShapeFactory = (ShapeFactory)oPart.ShapeFactory;
			Pad oPad = oShapeFactory.AddNewPad ( oCurrentSketch,  iBearingLength );
			OriginElements originElements1 = oPart.OriginElements;
			Reference oRefPlaneXY = oPart.CreateReferenceFromGeometry( oPart.OriginElements.PlaneXY );
			RectPattern rectPattern1 = oShapeFactory.AddNewRectPattern(oPad,iNumberOfCylinders+1,1,iCylinderSpacing,0,1,1,oRefPlaneXY,oRefPlaneXY,true,true,0);
			iCurrentLevel =  iBearingLength;
		}
	
		public static void CreateCam(double angle)
		{
			double dRad = angle*dPi/180;
			double dDSin1 = iCircle1Rad*Math.Sin(dRad);
			double dDCos1 = iCircle1Rad*Math.Cos(dRad);
			double dDSin2 = iCircle2Rad*Math.Sin(dRad);
			double dDCos2 = iCircle2Rad*Math.Cos(dRad);
			double dCSin = iCircleDist*Math.Sin(dRad);
			double dCCos = iCircleDist*Math.Cos(dRad);
			oCurrentSketch = oPartBody.Sketches.Add ( oPlaneYZ );
			Factory2D oFactory2D = oCurrentSketch.OpenEdition();
			double dRad1 = dRad - dPi/4;
			double dRad2 = dRad + dPi/4;
			oCurrentLine1 = oFactory2D.CreateLine( iCenterX - dDSin1, iCenterY + dDCos1, iCenterX + dCCos + dDSin2,  iCenterY - dCSin + dDCos2);
			oCurrentLine2 = oFactory2D.CreateLine( iCenterX + dDSin1, iCenterY - dDCos1, iCenterX + dCCos - dDSin2,  iCenterY - dCSin - dDCos2 );
			oCurrentCircle1 = oFactory2D.CreateCircle( iCenterX, iCenterY, iCircle1Rad,   dRad2,    dRad1);
			oCurrentCircle2 = oFactory2D.CreateCircle( iCenterX + dCCos, iCenterY + dCSin, iCircle2Rad,   dRad1,    dRad2);
			Reference oRefLine1 = oPart.CreateReferenceFromObject(oCurrentLine1);
			Reference oRefCircle1 = oPart.CreateReferenceFromObject(oCurrentCircle1);
			Reference oRefLine2 = oPart.CreateReferenceFromObject(oCurrentLine2);
			Reference oRefCircle2 = oPart.CreateReferenceFromObject(oCurrentCircle2);
			Reference oRefLine1StartPt = oPart.CreateReferenceFromObject(oCurrentLine1.StartPoint);
			Reference oRefLine1EndPt = oPart.CreateReferenceFromObject(oCurrentLine1.EndPoint);
			Reference oRefLine2StartPt = oPart.CreateReferenceFromObject(oCurrentLine2.StartPoint);
			Reference oRefLine2EndPt = oPart.CreateReferenceFromObject(oCurrentLine2.EndPoint);
			Reference oRefCircle1StartPt = oPart.CreateReferenceFromObject(oCurrentCircle1.StartPoint);
			Reference oRefCircle1EndPt = oPart.CreateReferenceFromObject(oCurrentCircle1.EndPoint);
			Reference oRefCircle2StartPt = oPart.CreateReferenceFromObject(oCurrentCircle2.StartPoint);
			Reference oRefCircle2EndPt = oPart.CreateReferenceFromObject(oCurrentCircle2.EndPoint);
			Constraints oConstraints = oCurrentSketch.Constraints;
			Constraint oConstraint = oConstraints.AddMonoEltCst(CatConstraintType.catCstTypeReference, oRefCircle1);
			oConstraint = oConstraints.AddMonoEltCst(CatConstraintType.catCstTypeReference, oRefCircle2);
			oConstraint = oConstraints.AddBiEltCst(CatConstraintType.catCstTypeTangency, oRefLine1, oRefCircle1);
			oConstraint = oConstraints.AddBiEltCst(CatConstraintType.catCstTypeTangency, oRefCircle2, oRefLine1);
			oConstraint = oConstraints.AddBiEltCst(CatConstraintType.catCstTypeOn, oRefCircle1StartPt, oRefLine1StartPt);
			oConstraint = oConstraints.AddBiEltCst(CatConstraintType.catCstTypeOn, oRefCircle2EndPt, oRefLine1EndPt);
			oConstraint = oConstraints.AddBiEltCst(CatConstraintType.catCstTypeTangency, oRefLine2, oRefCircle1);
			oConstraint = oConstraints.AddBiEltCst(CatConstraintType.catCstTypeTangency, oRefLine2, oRefCircle2);
			oConstraint = oConstraints.AddBiEltCst(CatConstraintType.catCstTypeOn, oRefCircle1EndPt, oRefLine2StartPt);
			oConstraint = oConstraints.AddBiEltCst(CatConstraintType.catCstTypeOn, oRefCircle2StartPt, oRefLine2EndPt);
			oCurrentSketch.CloseEdition();
			ShapeFactory oShapeFactory = (ShapeFactory)oPart.ShapeFactory;
			Pad oPad = oShapeFactory.AddNewPad ( oCurrentSketch,  iCamThickness + iCurrentLevel );
			oPad.SecondLimit.Dimension.Value = iCurrentLevel*-1;

			
		}
	
		public static void CreateCylinder(double thickness, double radius)
		{
			oCurrentSketch = oPartBody.Sketches.Add ( oPlaneYZ );
			Factory2D oFactory2D = (Factory2D)oCurrentSketch.OpenEdition();
			oCurrentCircle1 = oFactory2D.CreateClosedCircle (iCenterX, iCenterY, radius);
			oCurrentSketch.CloseEdition();
			ShapeFactory oShapeFactory = (ShapeFactory)oPart;
			Pad oPad = oShapeFactory.AddNewPad ( oCurrentSketch,  thickness + iCurrentLevel );
			oPad.SecondLimit.Dimension.Value = iCurrentLevel*-1;
			iCurrentLevel = iCurrentLevel + thickness;
		}
		
		public static void CreateCamSet(double angle)
		{
			CreateCam(angle);
			iCurrentLevel = iCurrentLevel + iCamThickness;
			CreateCylinder(iPinLength, iPinDiam);
			CreateCam(angle);
			iCurrentLevel = iCurrentLevel + iBearingLength + iCamThickness;
	
	
		}
	}
}