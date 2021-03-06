﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ToxicRagers.CarmageddonReincarnation.Helpers;
using ToxicRagers.Generics;
using ToxicRagers.Helpers;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public enum Troolean
    {
        Unset = -1,
        False = 0,
        True = 1
    }

    public class MT2 : Material
    {
        protected MT2 coreDefaults;

        protected XElement xml;

        protected string name;
        protected string basedOffOf;

        protected Troolean bDoubleSided = Troolean.Unset;
        protected Troolean bCastsShadows = Troolean.Unset;
        protected Troolean bFogEnabled = Troolean.Unset;
        protected Troolean bReceivesShadows = Troolean.Unset;
        protected Troolean bTranslucent = Troolean.Unset;

        protected Troolean bWalkable = Troolean.Unset;
        protected Troolean bPanickable = Troolean.Unset;
        protected Troolean bSitable = Troolean.Unset;
        protected Troolean bUnpickable = Troolean.Unset;

        protected Troolean bNeedsWorldLightDir = Troolean.Unset;
        protected Troolean bNeedsWorldSpaceVertexNormal = Troolean.Unset;
        protected Troolean bNeedsWorldEyePos = Troolean.Unset;
        protected Troolean bNeedsWorldVertexPos = Troolean.Unset;
        protected Troolean bNeedsLightingSpaceVertexNormal = Troolean.Unset;
        protected Troolean bNeedsVertexColour = Troolean.Unset;
        protected Troolean bNeedsLocalCubeMap = Troolean.Unset;
        protected Troolean bNeedsSeperateObjectColour = Troolean.Unset;

        protected string diffuse;
        protected string substance;

        protected VegetationAnimation vegetationAnimation;

        protected List<TextureCoordSource> textureCoordSources;
        protected List<Sampler> samplers;

        protected Vector3 alphaCutOff = Vector3.Zero;
        protected Vector3 multiplier = Vector3.Zero;
        protected Vector3 emissiveLight = Vector3.Zero;
        protected Vector3 emissiveFactor = Vector3.Zero;
        protected Vector3 emissiveColour = Vector3.Zero;
        protected Vector3 reflectionMultiplier = Vector3.Zero;
        protected Vector3 reflectionBluryness = Vector3.Zero;
        protected Vector3 fresnelR0 = Vector3.Zero;

        [Ignore]
        public MT2 Defaults
        {
            get { return coreDefaults; }
            set { coreDefaults = value; }
        }

        [Ignore]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Ignore]
        public string BasedOffOf
        {
            get { return basedOffOf; }
            set { basedOffOf = value; }
        }

        [Ignore]
        public string Texture
        {
            get { return diffuse; }
        }

        public Troolean DoubleSided
        {
            get { return bDoubleSided; }
            set { bDoubleSided = value; }
        }

        public Troolean CastsShadows
        {
            get { return bCastsShadows; }
            set { bCastsShadows = value; }
        }

        public Troolean ReceivesShadows
        {
            get { return bReceivesShadows; }
            set { bReceivesShadows = value; }
        }

        public Troolean Translucent
        {
            get { return bTranslucent; }
            set { bTranslucent = value; }
        }

        public Troolean Walkable
        {
            get { return bWalkable; }
            set { bWalkable = value; }
        }

        public Troolean Sitable
        {
            get { return bSitable; }
            set { bSitable = value; }
        }

        public Troolean Unpickable
        {
            get { return bUnpickable; }
            set { bUnpickable = value; }
        }

        public Troolean FogEnabled
        {
            get { return bFogEnabled; }
            set { bFogEnabled = value; }
        }

        public Troolean NeedsWorldSpaceVertexNormal
        {
            get { return bNeedsWorldSpaceVertexNormal; }
            set { bNeedsWorldSpaceVertexNormal = value; }
        }

        public Troolean NeedsWorldEyePos
        {
            get { return bNeedsWorldEyePos; }
            set { bNeedsWorldEyePos = value; }
        }

        public Troolean NeedsWorldVertexPos
        {
            get { return bNeedsWorldVertexPos; }
            set { bNeedsWorldVertexPos = value; }
        }

        public Troolean NeedsWorldLightDir
        {
            get { return bNeedsWorldLightDir; }
            set { bNeedsWorldLightDir = value; }
        }

        public Troolean NeedsLightingSpaceVertexNormal
        {
            get { return bNeedsLightingSpaceVertexNormal; }
            set { bNeedsLightingSpaceVertexNormal = value; }
        }

        public Troolean NeedsVertexColour
        {
            get { return bNeedsVertexColour; }
            set { bNeedsVertexColour = value; }
        }

        public Troolean NeedsLocalCubeMap
        {
            get { return bNeedsLocalCubeMap; }
            set { bNeedsLocalCubeMap = value; }
        }

        public Troolean NeedsSeperateObjectColour
        {
            get { return bNeedsSeperateObjectColour; }
            set { bNeedsSeperateObjectColour = value; }
        }

        public Troolean Panickable
        {
            get { return bPanickable; }
            set { bPanickable = value; }
        }

        public Single AlphaCutOff
        {
            get { return alphaCutOff.X; }
            set { alphaCutOff.X = value; }
        }

        public Vector3 EmissiveLight
        {
            get { return multiplier; }
            set { multiplier = value; }
        }

        public Single EmissiveFactor
        {
            get { return emissiveFactor.X; }
            set { emissiveFactor.X = value; }
        }

        public Vector3 Emissive_Colour
        {
            get { return emissiveColour; }
            set { emissiveColour = value; }
        }

        public Single ReflectionBluryness
        {
            get { return reflectionBluryness.X; }
            set { reflectionBluryness.X = value; }
        }

        public Vector3 ReflectionMultiplier
        {
            get { return multiplier; }
            set { multiplier = value; }
        }

        public Single Fresnel_R0
        {
            get { return fresnelR0.X; }
            set { fresnelR0.X = value; }
        }

        [Ignore]
        public List<TextureCoordSource> TextureCoordSources
        {
            get { return textureCoordSources; }
            set { textureCoordSources = value; }
        }

        [Ignore]
        public List<Sampler> Samplers
        {
            get { return samplers; }
            set { samplers = value; }
        }

        public MT2()
        {
            textureCoordSources = new List<TextureCoordSource>();
            samplers = new List<Sampler>();
        }

        public MT2(XElement xml)
            : this()
        {
            this.xml = xml;

            var dblSided = xml.Descendants("DoubleSided").FirstOrDefault();
            var castsShads = xml.Descendants("CastsShadows").FirstOrDefault();
            var recShads = xml.Descendants("ReceivesShadows").FirstOrDefault();
            var fog = xml.Descendants("FogEnabled").FirstOrDefault();
            var trans = xml.Descendants("Translucent").FirstOrDefault();
            var walk = xml.Descendants("Walkable").FirstOrDefault();
            var panic = xml.Descendants("Panickable").FirstOrDefault();
            var sit = xml.Descendants("Sitable").FirstOrDefault();
            var pick = xml.Descendants("Unpickable").FirstOrDefault();
            var needWSVN = xml.Descendants("NeedsWorldSpaceVertexNormal").FirstOrDefault();
            var needWEP = xml.Descendants("NeedsWorldEyePos").FirstOrDefault();
            var needWVP = xml.Descendants("NeedsWorldVertexPos").FirstOrDefault();
            var needWLD = xml.Descendants("NeedsWorldLightDir").FirstOrDefault();
            var needLSVN = xml.Descendants("NeedsLightingSpaceVertexNormal").FirstOrDefault();
            var needVC = xml.Descendants("NeedsVertexColour").FirstOrDefault();
            var needLCM = xml.Descendants("NeedsLocalCubeMap").FirstOrDefault();
            var needSOC = xml.Descendants("NeedsSeperateObjectColour").FirstOrDefault();

            if (dblSided != null) { bDoubleSided = (dblSided.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (castsShads != null) { bCastsShadows = (castsShads.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (recShads != null) { bReceivesShadows = (recShads.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (fog != null) { bFogEnabled = (fog.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (trans != null) { bTranslucent = (trans.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (walk != null) { bWalkable = (walk.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (panic != null) { bPanickable = (panic.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (sit != null) { bSitable = (sit.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (pick != null) { bUnpickable = (pick.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (needWSVN != null) { bNeedsWorldSpaceVertexNormal = (needWSVN.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (needWEP != null) { bNeedsWorldEyePos = (needWEP.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (needWVP != null) { bNeedsWorldVertexPos = (needWVP.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (needWLD != null) { bNeedsWorldLightDir = (needWLD.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (needLSVN != null) { bNeedsLightingSpaceVertexNormal = (needLSVN.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (needVC != null) { bNeedsVertexColour = (needVC.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (needLCM != null) { bNeedsLocalCubeMap = (needLCM.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }
            if (needSOC != null) { bNeedsSeperateObjectColour = (needSOC.Attribute("Value").Value.ToLower() == "true" ? Troolean.True : Troolean.False); }

            var mult = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Multiplier").FirstOrDefault();
            var emml = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "EmissiveLight").FirstOrDefault();
            var emmf = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "EmissiveFactor").FirstOrDefault();
            var emmc = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Emissive_Colour").FirstOrDefault();
            var refm = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "ReflectionMultiplier").FirstOrDefault();
            var fres = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "Fresnel_R0").FirstOrDefault();
            var refb = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "ReflectionBluryness").FirstOrDefault();
            var alph = xml.Descendants("Constant").Where(e => e.Attribute("Alias").Value == "AlphaCutOff").FirstOrDefault();

            if (mult != null) { multiplier = ReadConstant(mult); }
            if (emml != null) { emissiveLight = ReadConstant(emml); }
            if (emmf != null) { emissiveFactor = ReadConstant(emmf); }
            if (emmc != null) { emissiveColour = ReadConstant(emmc); }
            if (refm != null) { reflectionMultiplier = ReadConstant(refm); }
            if (fres != null) { fresnelR0 = ReadConstant(fres); }
            if (refb != null) { reflectionBluryness = ReadConstant(refb); }
            if (alph != null) { alphaCutOff = ReadConstant(alph); }

            var vegetation = xml.Descendants("VegetationAnimation").FirstOrDefault();

            if (vegetation != null) { vegetationAnimation = VegetationAnimation.CreateFromElement(vegetation); }

            foreach (var textureCoordSource in xml.Descendants("TextureCoordSource"))
            {
                textureCoordSources.Add(TextureCoordSource.CreateFromElement(textureCoordSource));
            }

            foreach (var sampler in xml.Descendants("Sampler"))
            {
                samplers.Add(Sampler.CreateFromElement(sampler));
            }

            // Rotator2D

            var sub = xml.Descendants("Substance").FirstOrDefault();

            if (sub != null) { substance = sub.Attribute("Name").Value; }
        }

        public static MT2 Load(string path)
        {
            Logger.LogToFile(Logger.LogLevel.Info, path);
            MT2 mt2 = new MT2(XElement.Load(path));

            if (mt2.xml.Descendants("BasedOffOf").Count() > 0)
            {
                string basedOffOf = mt2.xml.Descendants("BasedOffOf").Attributes("Name").First().Value;

                mt2 = (MT2)Activator.CreateInstance(Type.GetType("ToxicRagers.CarmageddonReincarnation.Formats.Materials." + basedOffOf, true, true), mt2.xml);
                mt2.basedOffOf = basedOffOf;
            }

            mt2.name = Path.GetFileNameWithoutExtension(path);

            return mt2;
        }

        public void Save(string path)
        {
            xml = new XElement("Material");

            xml.Add(
                new XElement("Version", "1.2"),
                new XElement("BasedOffOf", new XAttribute("Name", basedOffOf))
            );

            foreach (var property in this.GetType().GetProperties())
            {
                object[] attributes = property.GetCustomAttributes(true);

                if (property.CanRead && !attributes.Any(a => a.GetType() == typeof(Ignore)))
                {
                    bool bRequired = attributes.Any(a => a.GetType() == typeof(Required));

                    switch (property.PropertyType.ToString().Split('.').Last())
                    {
                        case "String":
                            {
                                string value = (string)property.GetValue(this, null);
                                if (value != null && value != (string)property.GetValue(coreDefaults, null))
                                {
                                    xml.Add(new XElement("Texture",
                                        new XAttribute("Alias", property.Name),
                                        new XAttribute("Value", value.ToString())
                                    ));
                                }
                            }
                            break;

                        case "Troolean":
                            {
                                Troolean value = (Troolean)property.GetValue(this, null);
                                if (value != Troolean.Unset && value != (Troolean)property.GetValue(coreDefaults, null)) { xml.Add(new XElement(property.Name, new XAttribute("Value", value.ToString()))); }
                            }
                            break;

                        case "Single":
                            {
                                Single value = (Single)property.GetValue(this, null);
                                if (value != (Single)property.GetValue(coreDefaults, null))
                                {
                                    xml.Add(new XElement("Constant",
                                        new XAttribute("Alias", property.Name),
                                        new XAttribute("Type", "float"),
                                        new XAttribute("Value", value.ToString(ToxicRagers.Culture))
                                    ));
                                }
                            }
                            break;

                        default:
                            Console.WriteLine("[{2}] {0} = {1}", property.Name, property.GetValue(this, null), property.PropertyType.ToString());
                            break;
                    }

                }
            }

            XMLWriter.Save(new XDocument(xml), path);
        }

        public static Vector3 ReadConstant(XElement constant)
        {
            Vector3 v = Vector3.Zero;

            switch (constant.Attribute("Type").Value.ToLower())
            {
                case "float3":
                    {
                        string[] s = constant.Attribute("Value").Value.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        int mult = (s.Length == 3 ? 1 : 0);
                        v.X = s[0 * mult].ToSingle();
                        v.Y = s[1 * mult].ToSingle();
                        v.Z = s[2 * mult].ToSingle();
                    }
                    break;

                case "float2":
                    {
                        string[] s = constant.Attribute("Value").Value.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        int mult = (s.Length == 2 ? 1 : 0);
                        v.X = s[0 * mult].ToSingle();
                        v.Y = s[1 * mult].ToSingle();
                    }
                    break;

                case "float":
                    v.X = constant.Attribute("Value").Value.ToSingle();
                    break;

                default:
                    throw new NotImplementedException(string.Format("Unknown Type: {0}", constant.Attribute("Type").Value));
            }

            return v;
        }
    }

    public class TextureCoordSource
    {
        string alias;
        bool bScrolling;
        Single scrollX;
        Single scrollY;
        Single scrollZ;
        bool bFlipBook;
        int framesX = 1;
        int framesY = 1;
        Single frameRate;
        bool bFlipBookSelect;
        int flipBookSelectFrame;
        bool bWaving;
        Single waveFrequenceU;
        Single waveFrequenceV;
        Single waveAmplitudeU;
        Single waveAmplitudeV;
        int uvStream;

        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }

        public int UVStream
        {
            get { return uvStream; }
            set { uvStream = value; }
        }

        public bool FlipBook
        {
            get { return bFlipBook; }
            set { bFlipBook = true; }
        }

        public static TextureCoordSource CreateFromElement(XElement element)
        {
            TextureCoordSource tcs = new TextureCoordSource();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "Alias":
                        tcs.alias = element.Attribute("Alias").Value;
                        break;

                    case "UVStream":
                        tcs.uvStream = element.Attribute("UVStream").Value.ToInt();
                        break;

                    case "Scrolling":
                        tcs.bScrolling = (element.Attribute("Scrolling").Value.ToLower() == "true");
                        break;

                    case "ScrollX":
                        tcs.scrollX = element.Attribute("ScrollX").Value.ToSingle();
                        break;

                    case "ScrollY":
                        tcs.scrollY = element.Attribute("ScrollY").Value.ToSingle();
                        break;

                    case "ScrollZ":
                        tcs.scrollZ = element.Attribute("ScrollZ").Value.ToSingle();
                        break;

                    case "FlipBook":
                        tcs.bFlipBook = (element.Attribute("FlipBook").Value.ToLower() == "true");
                        break;

                    case "FramesX":
                        tcs.framesX = element.Attribute("FramesX").Value.ToInt();
                        break;

                    case "FramesY":
                        tcs.framesY = element.Attribute("FramesY").Value.ToInt();
                        break;

                    case "FrameRate":
                        tcs.frameRate = element.Attribute("FrameRate").Value.ToSingle();
                        break;

                    case "FlipBookSelect":
                        tcs.bFlipBookSelect = (element.Attribute("FlipBookSelect").Value.ToLower() == "true");
                        break;

                    case "FlipBookSelectFrame":
                        tcs.flipBookSelectFrame = element.Attribute("FlipBookSelectFrame").Value.ToInt();
                        break;

                    case "Waving":
                        tcs.bWaving = (element.Attribute("Waving").Value.ToLower() == "true");
                        break;

                    case "WaveFrequenceU":
                        tcs.waveFrequenceU = element.Attribute("WaveFrequenceU").Value.ToSingle();
                        break;

                    case "WaveFrequenceV":
                        tcs.waveFrequenceV = element.Attribute("WaveFrequenceV").Value.ToSingle();
                        break;

                    case "WaveAmplitudeU":
                        tcs.waveAmplitudeU = element.Attribute("WaveAmplitudeU").Value.ToSingle();
                        break;

                    case "WaveAmplitudeV":
                        tcs.waveAmplitudeV = element.Attribute("WaveAmplitudeV").Value.ToSingle();
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Unknown Attribute: {0}", attribute.Name.LocalName));
                }
            }

            return tcs;
        }
    }

    public class Sampler
    {
        public enum Filter
        {
            Anisotropic,
            Linear,
            Point,
            None
        }

        public enum Address
        {
            Clamp,
            Wrap
        }

        public enum Usage
        {
            DiffuseAlbedo,
            TangentSpaceNormals,
            SpecAlbedo,
            SpecColour,
            SpecMask,
            SpecPower
        }

        string alias;
        string type = "2D";
        Filter minFilter = Filter.Linear;
        int maxAnisotropy = 4;
        Filter mipFilter = Filter.Linear;
        Filter magFilter = Filter.Linear;
        Single mipLevelBias;
        Address addressU = Address.Wrap;
        Address addressV = Address.Wrap;
        Address addressW;
        bool bsRGBRead;
        Usage usageRGB;
        Usage usageAlpha;

        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }

        public Filter MinFilter
        {
            get { return minFilter; }
            set { minFilter = value; }
        }

        public Filter MipFilter
        {
            get { return mipFilter; }
            set { mipFilter = value; }
        }

        public Filter MagFilter
        {
            get { return magFilter; }
            set { magFilter = value; }
        }

        public Usage UsageRGB
        {
            get { return usageRGB; }
            set { usageRGB = value; }
        }

        public Usage UsageAlpha
        {
            get { return usageAlpha; }
            set { usageAlpha = value; }
        }

        public bool sRGBRead
        {
            get { return bsRGBRead; }
            set { bsRGBRead = value; }
        }

        public static Sampler CreateFromElement(XElement element)
        {
            Sampler s = new Sampler();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "Alias":
                        s.alias = element.Attribute("Alias").Value;
                        break;

                    case "Type":
                        s.type = element.Attribute("Type").Value;
                        break;

                    case "MinFilter":
                        s.minFilter = element.Attribute("MinFilter").Value.ToEnum<Filter>();
                        break;

                    case "MaxAnisotropy":
                        s.maxAnisotropy = element.Attribute("MaxAnisotropy").Value.ToInt();
                        break;

                    case "MipFilter":
                        s.mipFilter = element.Attribute("MipFilter").Value.ToEnum<Filter>();
                        break;

                    case "MipLevelBias":
                        s.mipLevelBias = element.Attribute("MipLevelBias").Value.ToSingle();
                        break;

                    case "MagFilter":
                        s.magFilter = element.Attribute("MagFilter").Value.ToEnum<Filter>();
                        break;

                    case "AddressU":
                        s.addressU = element.Attribute("AddressU").Value.ToEnum<Address>();
                        break;

                    case "AddressV":
                        s.addressV = element.Attribute("AddressV").Value.ToEnum<Address>();
                        break;

                    case "AddressW":
                        s.addressW = element.Attribute("AddressW").Value.ToEnum<Address>();
                        break;

                    case "sRGBRead":
                        s.bsRGBRead = (element.Attribute("sRGBRead").Value.ToLower() == "true");
                        break;

                    case "UsageRGB":
                        s.usageRGB = element.Attribute("UsageRGB").Value.ToEnum<Usage>();
                        break;

                    case "UsageAlpha":
                        s.usageAlpha = element.Attribute("UsageAlpha").Value.ToEnum<Usage>();
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Unknown Attribute: {0}", attribute.Name.LocalName));
                }
            }

            return s;
        }
    }

    public class VegetationAnimation
    {
        bool bEnabled;
        Single branchAmplitude;
        Single detailAmplitude;
        Single detailFrequency;
        Single bendScale;

        public static VegetationAnimation CreateFromElement(XElement element)
        {
            VegetationAnimation va = new VegetationAnimation();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "Enabled":
                        va.bEnabled = (element.Attribute("Enabled").Value.ToLower() == "true");
                        break;

                    case "BranchAmplitude":
                        va.branchAmplitude = element.Attribute("BranchAmplitude").Value.ToSingle();
                        break;

                    case "DetailAmplitude":
                        va.detailAmplitude = element.Attribute("DetailAmplitude").Value.ToSingle();
                        break;

                    case "DetailFrequency":
                        va.detailFrequency = element.Attribute("DetailFrequency").Value.ToSingle();
                        break;

                    case "BendScale":
                        va.bendScale = element.Attribute("BendScale").Value.ToSingle();
                        break;

                    default:
                        throw new NotImplementedException(string.Format("Unknown Attribute: {0}", attribute.Name.LocalName));
                }
            }

            return va;
        }
    }

    public class Required : Attribute { }
    public class Ignore : Attribute { }
}