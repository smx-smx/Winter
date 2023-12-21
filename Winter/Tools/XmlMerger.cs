using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Smx.Winter.Tools
{
    public class XmlMerger
    {
        public XDocument Merged { get; private set; }
        public XmlMerger()
        {
            Merged = new XDocument();
        }

        private void ApplyAttributes(XElement reference, XElement patch)
        {
            var patchAttrs = patch.Attributes()
                .ExceptBy(reference.Attributes().Select(a => a.Name),
                a => a.Name
            );

            foreach (var a in patchAttrs)
            {
                reference.SetAttributeValue(a.Name, a.Value);
            }
        }

        private void ApplyElement(XElement reference, XElement patch)
        {
            ApplyAttributes(reference, patch);

            var refElements = reference.Elements();
            var patchElements = patch.Elements();

            foreach(var pel in patchElements)
            {
                var candidates = refElements.Where(e => e.Name == pel.Name);
                if (!candidates.Any())
                {
                    reference.Add(pel);
                    return;
                }

                // how many?
                var isMany = patchElements.Count(x => x.Name == pel.Name) > 1;
                if(isMany && candidates.Count() < 2)
                {
                    // convert from one to many
                    reference.Add(pel);
                    return;
                }

                // add missing attributes
                var first = candidates.First();

                // traverse
                ApplyElement(first, pel);
            }
        }


        public void MergeFromPath(string xmlPath)
        {
            var patch = XDocument.Load(xmlPath);
            if (patch == null || patch.Root == null) return;

            if(Merged.Root == null)
            {
                Merged.Add(new XElement(patch.Root.Name));
            }

            ApplyElement(Merged.Root, patch.Root);
            //merged.Save(Console.Out);
            //Console.ReadLine();
        }

        public static void ToolMain(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: [manifestsDir][outFile]");
                Environment.Exit(1);
            }
            var manifestsPath = args[0];
            var outFile = args[1];

            var xm = new XmlMerger();
            
            var manifests = Directory.EnumerateFiles(manifestsPath, "*.manifest");
            var i = 0;
            var nManifests = manifests.Count();
            foreach (var manifest in manifests)
            {
                Console.WriteLine($"{++i}/{nManifests}: {manifest}");
                xm.MergeFromPath(manifest);
            }
            
            xm.Merged.Save(Console.Out);
            xm.Merged.Save(outFile);
        }
    }
}
