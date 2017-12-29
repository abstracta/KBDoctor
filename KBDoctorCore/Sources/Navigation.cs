﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Xsl;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

using Artech.Genexus.Common;
using Artech.Architecture.Common.Collections;
using Artech.Architecture.Common.Objects;
using Artech.Architecture.Common.Services;
using Artech.Architecture.UI.Framework.Services;
using Artech.Genexus.Common.Helpers;
using Artech.Genexus.Common.Objects;
using Artech.Udm.Framework.References;
using Artech.Architecture.UI.Framework.Helper;
using Artech.Common.Framework.Commands;
using Artech.Genexus.Common.Entities;
using Artech.Genexus.Common.Collections;
using Artech.Architecture.Common.Descriptors;

namespace Concepto.Packages.KBDoctorCore.Sources
{
    static class Navigation
    {
        internal static void PrepareComparerNavigation(KnowledgeBase KB, IOutputService output)
        {
            string title = "KBDoctor - Prepare Comparer Navigation Files";
            output.StartSection(title);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            string directoryArg = Utility.NvgComparerDirectory(KB);
            string fechahora = String.Format("{0:yyyy-MM-dd-HHmm}", DateTime.Now);
            string newDir = directoryArg + @"\NVG-" + fechahora + @"\";

            try
            {
                Directory.CreateDirectory(newDir);
                Utility.WriteXSLTtoDir(KB);
                foreach (string d in Directory.GetDirectories(Utility.SpcDirectory(KB), "NVG", System.IO.SearchOption.AllDirectories))
                {
                    output.AddLine("Procesando directorio: " + d);
                    string generator = d.Replace(Utility.SpcDirectory(KB), "");
                    generator = generator.Replace("NVG_", "");
                    generator = @"\" + generator.Replace(@"\", "_") + "_";
                    generator = generator.Replace("NVG_", "");

                    ProcesoDir(KB, d, newDir, generator, output);
                }

                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                // Format and display the TimeSpan value. 
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);

                output.AddLine(title + " elepsed time: " + elapsedTime);
                output.EndSection(title, true);
            }
            catch (Exception e){
                output.AddErrorLine(e.Message);
            }
        }

        private static void ProcesoDir(KnowledgeBase KB, string directoryArg, string newDir, string generator, IOutputService output)
        {
            string outputFile = KB.UserDirectory + @"\KBdoctorEv2.xslt";
            XslTransform xslTransform = new XslTransform();

            output.AddLine("Cargando archivo xslt: " + outputFile);
            xslTransform.Load(outputFile);
            output.AddLine("Archivo xslt cargado correctamente.");
            string fileWildcard = @"*.xml";
            var searchSubDirsArg = System.IO.SearchOption.AllDirectories;
            string[] xFiles = System.IO.Directory.GetFiles(directoryArg, fileWildcard, searchSubDirsArg);

            foreach (string x in xFiles)
            {
                if (!Path.GetFileNameWithoutExtension(x).StartsWith("Gx0"))
                {
                    output.AddLine("Procesando archivo: " + x);
                    string xTxt = newDir + generator + Path.GetFileNameWithoutExtension(x) + ".nvg";

                    string xmlstring = Utility.AddXMLHeader(x);

                    string newXmlFile = x.Replace(".xml", ".xxx");
                    File.WriteAllText(newXmlFile, xmlstring);

                    xslTransform.Transform(newXmlFile, xTxt);
                    File.Delete(newXmlFile);
                }
            }
        }

        internal static bool CompareLastNVGDirectories (KnowledgeBase KB, IOutputService output)
        {
            try
            {
                bool isSuccess = true;
                string pathNvg = Path.Combine(Utility.SpcDirectory(KB), "NvgComparer");
                string fileWildcard = @"*.*";
                string[] Files = Directory.GetDirectories(pathNvg, fileWildcard);
                string[] Last2directories = GetLast2Directorys(Files, output);
                int cant_error = 0;
                if (Last2directories == null || Last2directories.Length != 2) {
                    output.AddErrorLine("Ocurrió un error procesando los directorios de navegaciones.");
                    output.AddErrorLine("Asegúrece de que existen al menos dos directorios con nombres en el formato válido (NVG-AAAA-MM-DD-HHMM)");
                }
                else
                {
                    output.AddLine("Se utilizarán los siguientes directorios para comparar:");
                    output.AddLine("-- " + Last2directories[0].ToString());
                    output.AddLine("-- " + Last2directories[1].ToString());
                    List<string> Diffs = EqualNavigationDirectories(Last2directories[0], Last2directories[1], output);
                    output.AddLine("-- Los directorios se procesaron correctamente.");
                    if (Diffs.Count > 0)
                    {
                        output.AddLine("-- Se encontraron diferencias en las navegaciones de los siguientes objetos:");
                        foreach (string x in Diffs) 
                        {
                            string[] objectnametype = Utility.ReadQnameTypeFromNVGFile(x, output);
                            string objtype = objectnametype[0];
                            string objmodule = objectnametype[1];
                            string objname = objectnametype[2];
                            KBObjectDescriptor kbod = KBObjectDescriptor.Get(objtype);
                            QualifiedName qname = new QualifiedName(objmodule,objname);
                            KBObject obj = KB.DesignModel.Objects.Get(kbod.Id,qname);
                            if (obj != null)
                            {
                                if (obj.Timestamp <= Utility.GetDateTimeNVGDirectory(Last2directories[1].ToString()))
                                {
                                    if (objmodule != "")
                                    {
                                        output.AddLine("-- ERROR " + objmodule + '.' + objname + " fue modificado en \t\t" + obj.Timestamp.ToString());
                                    }
                                    else
                                    {
                                        output.AddLine("-- ERROR " + objname + " fue modificado en \t\t" + obj.Timestamp.ToString());
                                    }
                                    
                                    isSuccess = false;
                                    cant_error++; 
                                }
                                else
                                {
                                    if (objmodule != "")
                                    {
                                        output.AddLine("-- -- OK " + objmodule + '.' + objname + " fue modificado en \t\t" + obj.Timestamp.ToString());
                                    }
                                    else {
                                        output.AddLine("-- -- OK " + objname + " fue modificado en \t\t" + obj.Timestamp.ToString());
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        output.AddLine("No se encontraron diferencias en las navegaciones");
                    }
                    
                }
                if (cant_error > 0){
                    output.AddErrorLine("Se encontraron " + cant_error + " errores en la comparación.");
                }
                return isSuccess;
            }
            catch (Exception e)
            {
                output.AddLine(e.Message);
                return false;
            }
        }

        private static string[] GetLast2Directorys(string[] Files, IOutputService output)
        {
            
            DateTime FechaMax = new DateTime(1830,1,1);
            DateTime FechaMaxSec = new DateTime(1830,1,1);
            string DirectoryMax = "";
            string DirectorySec = "";
            string[] last2directories = new string[2];
            if (Files.Length <= 1)
            {
                output.AddErrorLine("Error: No existen 2 directorios de navegaciones para comparar");
                return last2directories;
            }
            foreach (string x in Files)
            {
                DateTime FechaX = Utility.GetDateTimeNVGDirectory(x);
                if (!DateTime.Equals(FechaX, new DateTime()))
                {
                    if (DateTime.Compare(FechaMax, FechaX) < 0)
                    {
                        FechaMaxSec = FechaMax;
                        DirectorySec = DirectoryMax;
                        FechaMax = FechaX;
                        DirectoryMax = x;
                    }
                    else
                    {
                        if (DateTime.Compare(FechaMaxSec, FechaX) < 0)
                        {
                            FechaMaxSec = FechaX;
                            DirectorySec = x;
                        }
                    }
                }
                else
                {
                    output.AddWarningLine("Error al intentar obtener la fecha del directorio: " + x);
                }
            }
            last2directories[0] = DirectoryMax;
            last2directories[1] = DirectorySec;
            return last2directories;
        }

        
        private static List<string> EqualNavigationDirectories(string pathSource, string pathNew, IOutputService output)
        {
            string fileWildcard = @"*.*";
            var searchSubDirsArg = System.IO.SearchOption.AllDirectories;
            string[] FilesDirectorySource = System.IO.Directory.GetFiles(pathSource, fileWildcard, searchSubDirsArg);
            List<string> Diffs = new List<string>();
            foreach (string fileSourcePath in FilesDirectorySource)
            {
                string fileNewPath = Path.Combine(pathNew, Path.GetFileName(fileSourcePath));
                if (File.Exists(fileNewPath))
                {
                    FileInfo fileNew = new FileInfo(fileNewPath);
                    FileInfo fileSource = new FileInfo(fileSourcePath);
                    if (!Utility.FilesAreEqual(fileSource, fileNew))
                    {
                        Diffs.Add(fileNewPath);
                    }
                }
                else
                {
                    output.AddLine("-- Nuevo: " + fileNewPath);
                }
            }
            return Diffs;
        }
    }
}