﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

using ToxicRagers.Helpers;

namespace ToxicRagers.Stainless.Formats
{
    public class MDL
    {
        [Flags]
        public enum Flags
        {
            USERData = 1,
            PREPAlternative = 2,
            USERSkinData = 4,
            Unknown8 = 8,
            Unknown16 = 16,
            PREPSkinData = 32
        }

        public static Dictionary<string, Version> SupportedVersions = new Dictionary<string, Version>
        {
            //{ "5.6", new Version(5,6)}, // iOS, Novadrome
            { "5.9", new Version(5,9)}, // iOS, Novadrome
            { "6.0", new Version(6,0)}, // iOS, Novadrome
            { "6.1", new Version(6,1)}, // Novadrome
            { "6.2", new Version(6,2)}  // C:R
        };

        int userFaceCount;
        int userVertexCount;
        int prepFaceCount;
        int prepVertexCount;
        Version version;
        string name;
        int checkSum;
        Flags flags;
        int prepDataSize;
        int fileSize;
        Flags userFlags;

        List<MDLMaterialGroup> meshes = new List<MDLMaterialGroup>();
        List<MDLFace> faces = new List<MDLFace>();
        List<MDLVertex> verts = new List<MDLVertex>();
        List<MDLUserVertexEntry> userVertexList = new List<MDLUserVertexEntry>();
        List<MDLUserFaceEntry> userFaceList = new List<MDLUserFaceEntry>();
        List<MDLBone> prepBoneList = new List<MDLBone>();
        List<int> ptouVertexLookup = new List<int>();
        List<int> ptouFaceLookup = new List<int>();

        List<MDLPrepSkinWeightLookup> prepVertSkinWeightLookup = new List<MDLPrepSkinWeightLookup>();

        MDLExtents extents = new MDLExtents();

        public string Name { get { return (name != null ? name : "Unknown Mesh"); } }
        public List<MDLMaterialGroup> Meshes { get { return meshes; } }
        public int PREPFaceCount { get { return prepFaceCount; } }
        public int USERFaceCount { get { return userFaceCount; } }
        public int PREPVertexCount { get { return prepVertexCount; } }
        public int USERVertexCount { get { return userVertexCount; } }

        public List<MDLFace> Faces { get { return faces; } set { faces = value; } }
        public List<MDLVertex> Vertices { get { return verts; } set { verts = value; } }

        public int Checksum { get { return checkSum; } set { checkSum = value; } }
        public Flags ModelFlags { get { return flags; } set { flags = value; } }
        public int PrepDataSize { get { return prepDataSize; } set { prepDataSize = value; } }

        public static MDL Load(string path)
        {
            FileInfo fi = new FileInfo(path);
            Logger.LogToFile(Logger.LogLevel.Info, "{0}", path);
            MDL mdl = new MDL();

            mdl.name = fi.Name.Replace(fi.Extension, "");

            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
            using (BinaryReader br = new BinaryReader(ms, Encoding.Default))
            {
                if (br.ReadByte() != 0x45||
                    br.ReadByte() != 0x23)
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "{0} isn't a valid MDL file", path);
                    return null;
                }

                byte minor = br.ReadByte();
                byte major = br.ReadByte();

                mdl.version = new Version(major, minor);

                if (!MDL.SupportedVersions.ContainsKey(mdl.version.ToString()))
                {
                    Logger.LogToFile(Logger.LogLevel.Error, "Unsupported MDL version: v{0}", mdl.version.ToString());
                    return null;
                }

                Logger.LogToFile(Logger.LogLevel.Info, "MDL v{0}", mdl.version.ToString());

                // TODO: v5.6
                // Ref : Novadrome_Demo\WADs\data\DATA\SFX\CAR_EXPLOSION\DEBPOOL\DEBPOOL.MDL
                // Ref : Carmageddon Mobile\Data_IOS\DATA\CONTENT\SFX\SHRAPNEL.MDL
                // 01 00 00 00 EE 02 00 00 02 00 00 00 04 00 00 00 01 00 00 00 49 33 35 3F 89 41 00 BF 18 B7 51 BA 89 41 00 BF 00 00 00 3F 52 49 9D 3A 00 00 00 3F 00 12 03 BA 18 B7 51 39 00 12 03 BA 01 00 66 69 72 65 70 6F 6F 6C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 17 B7 51 39 17 B7 D1 38 00 00 80 3F 17 B7 D1 B8 00 00 00 00 03 00 00 00 02 00 00 00 01 00 00 00 17 B7 51 39 17 B7 D1 B8 00 00 80 3F 17 B7 D1 38 04 00 00 00 00 00 00 3F 17 B7 D1 38 00 00 00 BF 17 B7 D1 38 00 00 80 3F 17 B7 D1 B8 00 00 80 3F 00 00 80 3F 00 00 00 00 00 00 00 00 80 80 80 FF 00 00 00 BF 17 B7 51 39 00 00 00 BF 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 80 80 80 FF 00 00 00 3F 17 B7 51 39 00 00 00 3F 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 80 80 80 FF 00 00 00 BF 17 B7 D1 38 00 00 00 3F 17 B7 D1 B8 00 00 80 3F 17 B7 D1 38 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 80 80 FF 01 00 00 00 00 00 17 B7 D1 38 00 00 00 00 00 00 00 00 04 00 00 00 04 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF

                mdl.checkSum = (int)br.ReadUInt32();

                mdl.flags = (Flags)br.ReadUInt32();
                Logger.LogToFile(Logger.LogLevel.Debug, "Flags {0}", (Flags)mdl.flags);

                mdl.prepDataSize = (int)br.ReadUInt32();    // PREP data size

                mdl.userFaceCount = (int)br.ReadUInt32();
                mdl.userVertexCount = (int)br.ReadUInt32();

                Logger.LogToFile(Logger.LogLevel.Debug, "USER Faces: {0}", mdl.userFaceCount);
                Logger.LogToFile(Logger.LogLevel.Debug, "USER Verts: {0}", mdl.userVertexCount);

                mdl.fileSize = (int)br.ReadUInt32();

                mdl.extents.Radius = br.ReadSingle();
                mdl.extents.Min = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                mdl.extents.Max = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                br.ReadBytes(12);   //  BoundingBox centre, Flummery auto calculates from min and max

                int materialCount = br.ReadInt16();

                Logger.LogToFile(Logger.LogLevel.Debug, "Material count: {0}", materialCount);

                for (int i = 0; i < materialCount; i++)
                {
                    string materialName;
                    int nameLength = -1;
                    int padding = -1;

                    if (mdl.version.Major < 6)
                    {
                        materialName = br.ReadBytes(32).ToName();
                    }
                    else
                    {
                        nameLength = (int)br.ReadInt32();
                        padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength + (mdl.version.Major == 6 && mdl.version.Minor > 0 ? 4 : 0);
                        materialName = br.ReadString(nameLength);
                        br.ReadBytes(padding);
                    }

                    mdl.meshes.Add(new MDLMaterialGroup(i, materialName));
                }

                // START PREP DATA
                mdl.prepFaceCount = (int)br.ReadUInt32();

                Logger.LogToFile(Logger.LogLevel.Debug, "PREP Faces: {0}", mdl.prepFaceCount);

                for (int i = 0; i < mdl.prepFaceCount; i++)
                {
                    var face = new MDLFace(
                        br.ReadUInt16(),        // Material index
                        br.ReadUInt16(),        // Material Flags
                        (int)br.ReadUInt32(),   // Vert index A
                        (int)br.ReadUInt32(),   // Vert index B
                        (int)br.ReadUInt32()    // Vert index C
                    );

                    mdl.faces.Add(face);

                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} : {1}", i, face);
                }

                mdl.prepVertexCount = (int)br.ReadUInt32();

                Logger.LogToFile(Logger.LogLevel.Debug, "PREP Verts: {0}", mdl.prepVertexCount);

                for (int i = 0; i < mdl.prepVertexCount; i++)
                {
                    var vert = new MDLVertex(
                        br.ReadSingle(),        // X
                        br.ReadSingle(),        // Y
                        br.ReadSingle(),        // Z
                        br.ReadSingle(),        // N.X
                        br.ReadSingle(),        // N.Y
                        br.ReadSingle(),        // N.Z
                        br.ReadSingle(),        // U
                        br.ReadSingle(),        // V
                        br.ReadSingle(),        // U2
                        br.ReadSingle(),        // V2
                        br.ReadByte(),          // R
                        br.ReadByte(),          // G
                        br.ReadByte(),          // B
                        br.ReadByte()           // A
                    );

                    mdl.verts.Add(vert);

                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} : {1}", i, vert);
                }

                int materialGroups = br.ReadUInt16();

                for (int i = 0; i < materialGroups; i++)
                {
                    var mesh = mdl.meshes[i];

                    br.ReadBytes(12);   // BoundingBox Centre, we recalculate it from Min and Max
                    mesh.Extents.Radius = br.ReadSingle();
                    mesh.Extents.Min = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    mesh.Extents.Max = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                    mesh.StripOffset = (int)br.ReadUInt32();
                    mesh.StripVertCount = (int)br.ReadUInt32();
                    int stripPointCount = (int)br.ReadUInt32();

                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} : {1} : {2}", mesh.StripOffset, mesh.StripVertCount, stripPointCount);

                    for (int j = 0; j < stripPointCount; j++)
                    {
                        uint index = br.ReadUInt32();
                        bool bDegenerate = ((index & 0x80000000) != 0);
                        index &= ~0x80000000;

                        mesh.StripList.Add(new MDLPoint((int)index + mesh.StripOffset, bDegenerate));

                        Logger.LogToFile(Logger.LogLevel.Debug, "{0} ] {1} : {2}", j, index, bDegenerate);
                    }

                    mesh.TriListOffset = (int)br.ReadUInt32();
                    mesh.TriListVertCount = (int)br.ReadUInt32();
                    int listPointCount = (int)br.ReadUInt32();

                    Logger.LogToFile(Logger.LogLevel.Debug, "{0} : {1} : {2}", mesh.TriListOffset, mesh.TriListVertCount, listPointCount);

                    for (int j = 0; j < listPointCount; j++)
                    {
                        uint index = br.ReadUInt32();

                        mesh.TriList.Add(new MDLPoint((int)index + mesh.TriListOffset));

                        Logger.LogToFile(Logger.LogLevel.Debug, "{0} ] {1}", j, index);
                    }
                }

                if (mdl.flags.HasFlag(Flags.PREPSkinData))
                {
                    Logger.LogToFile(Logger.LogLevel.Debug, "Processing PREP skin data");

                    int bodyPartCount = br.ReadUInt16();
                    int maxBonesPerVertex = br.ReadUInt16();
                    int rootBoneIndex = br.ReadUInt16();
                    var boneNames = br.ReadStrings(bodyPartCount);

                    Logger.LogToFile(Logger.LogLevel.Debug, "Body Part Count: {0}. Max Bones per Vertex : {1}. Root Bone Index : {2}", bodyPartCount, maxBonesPerVertex, rootBoneIndex);

                    for (int i = 0; i < bodyPartCount; i++)
                    {
                        var bone = new MDLBone();

                        bone.Name = boneNames[i];
                        bone.MinExtents = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());   // Min and Max bone local space
                        bone.MaxExtents = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        bone.Offset = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());       // Offset is in parents local space

                        bone.Parent = br.ReadByte();
                        bone.Child = br.ReadByte();
                        bone.Sibling = br.ReadByte();

                        mdl.prepBoneList.Add(bone);
                    }

                    for (int i = 0; i < bodyPartCount; i++)
                    {
                        mdl.prepBoneList[i].Rotation = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        mdl.prepBoneList[i].Position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        br.ReadBytes(4);

                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}) {1}", i, mdl.prepBoneList[i].Name);
                        Logger.LogToFile(Logger.LogLevel.Debug, "P{0} C{1} S{2}", mdl.prepBoneList[i].Parent, mdl.prepBoneList[i].Child, mdl.prepBoneList[i].Sibling);
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}", mdl.prepBoneList[i].MinExtents);
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}", mdl.prepBoneList[i].MaxExtents);
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}", mdl.prepBoneList[i].Offset);
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}", mdl.prepBoneList[i].Rotation);
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0}", mdl.prepBoneList[i].Position);
                        Logger.LogToFile(Logger.LogLevel.Debug, "");
                    }

                    Logger.LogToFile(Logger.LogLevel.Debug, "PREP skin vert weight table");

                    for (int i = 0; i < mdl.prepVertexCount; i++)
                    {
                        int weightCount = br.ReadUInt16();
                        br.ReadBytes(2);
                        int weightOffset = (int)br.ReadUInt32();

                        mdl.prepVertSkinWeightLookup.Add(new MDLPrepSkinWeightLookup { Count = weightCount, Index = weightOffset });

                        Logger.LogToFile(Logger.LogLevel.Debug, "{0,5}]  {1} @ {2}", i, weightCount, weightOffset);
                    }

                    int prepSkinWeightCount = (int)br.ReadUInt32();

                    Logger.LogToFile(Logger.LogLevel.Debug, "PREP Skin Weight Count: {0}", prepSkinWeightCount);

                    for (int i = 0; i < prepSkinWeightCount; i++)
                    {
                        int boneIndex = br.ReadUInt16();
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0,5}] {1}", i, boneIndex);
                    }

                    for (int i = 0; i < prepSkinWeightCount; i++)
                    {
                        Single weight = br.ReadSingle();
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0,5}] {1}", i, weight);
                    }
                }
                // END PREP DATA

                // START USER DATA
                if (mdl.flags.HasFlag(Flags.USERData))
                {
                    mdl.userFlags = (Flags)br.ReadUInt32();

                    // v5.6 successfully parses from this point down

                    Logger.LogToFile(Logger.LogLevel.Debug, "USER vertex list with index count");
                    for (int i = 0; i < mdl.userVertexCount; i++)
                    {
                        mdl.userVertexList.Add(new MDLUserVertexEntry(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), (int)br.ReadUInt32()));
                    }

                    Logger.LogToFile(Logger.LogLevel.Debug, "USER face data");
                    for (int i = 0; i < mdl.userFaceCount; i++)
                    {
                        if (mdl.version.Major == 5 && mdl.version.Minor == 6)
                        {
                            br.ReadBytes(133);
                        }
                        else
                        {
                            mdl.userFaceList.Add(
                                new MDLUserFaceEntry(
                                        br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), // plane equation
                                        new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),     // vertex[0] normal
                                        new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),     // vertex[1] normal
                                        new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),     // vertex[2] normal
                                        (int)br.ReadUInt32(),                                               // material index
                                        (int)br.ReadUInt32(),                                               // smoothing group
                                        (int)br.ReadUInt32(),                                               // vertex[0]
                                        (int)br.ReadUInt32(),                                               // vertex[1]
                                        (int)br.ReadUInt32(),                                               // vertex[2]
                                        br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte(),         // colour[0] RGBA
                                        br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte(),         // colour[1] RGBA
                                        br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte(),         // colour[2] RGBA
                                        new Vector2(br.ReadSingle(), br.ReadSingle()),                      // uv[0]
                                        new Vector2(br.ReadSingle(), br.ReadSingle()),                      // uv2[0]
                                        new Vector2(br.ReadSingle(), br.ReadSingle()),                      // uv[1]
                                        new Vector2(br.ReadSingle(), br.ReadSingle()),                      // uv2[1]
                                        new Vector2(br.ReadSingle(), br.ReadSingle()),                      // uv[2]
                                        new Vector2(br.ReadSingle(), br.ReadSingle()),                      // uv2[2]
                                        br.ReadByte(),                                                      // flags
                                        (int)br.ReadUInt32()                                                // application specific flags
                                ) // 137 bytes
                            );
                        }
                    }

                    Logger.LogToFile(Logger.LogLevel.Debug, "PREP to USER face lookup");
                    for (int i = 0; i < mdl.prepFaceCount; i++)
                    {
                        mdl.ptouFaceLookup.Add((int)br.ReadUInt32());
                    }

                    int prepVertexMapCount = (int)br.ReadUInt32();
                    Logger.LogToFile(Logger.LogLevel.Debug, "PREP to USER vertex lookup");

                    for (int i = 0; i < prepVertexMapCount; i++)
                    {
                        mdl.ptouVertexLookup.Add((int)br.ReadUInt32());
                    }

                    if (mdl.userFlags.HasFlag(Flags.USERSkinData))
                    {
                        Logger.LogToFile(Logger.LogLevel.Debug, "Processing USER skin data");

                        int boneCount = br.ReadUInt16();

                        Logger.LogToFile(Logger.LogLevel.Debug, "Bone count: {0}", boneCount);
                        for (int i = 0; i < boneCount; i++)
                        {
                            string boneName = br.ReadString(32);
                            short parentBoneIndex = br.ReadInt16();
                            Matrix3D boneTransform = new Matrix3D(
                                                            br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                            br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                            br.ReadSingle(), br.ReadSingle(), br.ReadSingle(),
                                                            br.ReadSingle(), br.ReadSingle(), br.ReadSingle()
                                                        );

                            Logger.LogToFile(Logger.LogLevel.Debug, "{0}) {1}", i, boneName);
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0}", parentBoneIndex);
                            Logger.LogToFile(Logger.LogLevel.Debug, "{0}", boneTransform);
                            Logger.LogToFile(Logger.LogLevel.Debug, "");
                        }

                        int userDataCount = (int)br.ReadUInt32();
                        Logger.LogToFile(Logger.LogLevel.Debug, "{0} == {1}", userDataCount, mdl.userVertexCount);

                        for (int i = 0; i < mdl.userVertexCount; i++)
                        {
                            int entryCount = br.ReadUInt16();

                            for (int j = 0; j < entryCount; j++)
                            {
                                int boneIndex = br.ReadUInt16();
                                Single weight = br.ReadSingle();
                                Vector3 vertexPosition = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                Logger.LogToFile(Logger.LogLevel.Debug, "{0}.{1}]  {2,2} : {3,6:0.00}% : {4}", i, j, boneIndex, (weight * 100.0f), vertexPosition);
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("no user data");
                }

                if (br.BaseStream.Position != br.BaseStream.Length) { Logger.LogToFile(Logger.LogLevel.Warning, "Still has data remaining (processed {0:x2} of {1:x2}", br.BaseStream.Position, br.BaseStream.Length); }
            }

            return mdl;
        }

        public MDLMaterialGroup GetMesh(int index)
        {
            return meshes[index];
        }

        public void Save(string path)
        {
            int nameLength, padding;
            this.CalculateExtents();

            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                bw.Write(new byte[] { 0x45, 0x23 });    // Magic Number
                bw.Write(new byte[] { 2, 6 });          // Version (6.2)

                bw.Write(new byte[] { 0, 0, 0, 0 });    // Checksum, to calculate
                bw.Write((int)this.flags);

                int prepDataSize = 4 + (this.faces.Count * 16) + 4 + (this.verts.Count * 44) + 2;

                for (int i = 0; i < this.meshes.Count; i++)
                {
                    var mesh = this.meshes[i];

                    prepDataSize += 52;
                    prepDataSize += (4 * mesh.StripList.Count);

                    prepDataSize += 12;
                    prepDataSize += (4 * mesh.TriList.Count);
                }

                bw.Write(prepDataSize);                 // PREP data size

                bw.Write(this.faces.Count);             // USER face count
                bw.Write(this.verts.Count);             // USER vert count

                bw.Write(0);                            // Back filled post save

                bw.Write(this.extents.Radius);
                bw.Write(this.extents.Min.X);
                bw.Write(this.extents.Min.Y);
                bw.Write(this.extents.Min.Z);
                bw.Write(this.extents.Max.X);
                bw.Write(this.extents.Max.Y);
                bw.Write(this.extents.Max.Z);
                bw.Write(this.extents.Centre.X);
                bw.Write(this.extents.Centre.Y);
                bw.Write(this.extents.Centre.Z);

                bw.Write((short)this.Meshes.Count);

                for (int i = 0; i < this.Meshes.Count; i++)
                {
                    nameLength = this.Meshes[i].Name.Length;
                    padding = (((nameLength / 4) + (nameLength % 4 > 0 ? 1 : 0)) * 4) - nameLength + 4;

                    bw.Write(nameLength);
                    bw.WriteString(this.Meshes[i].Name);
                    bw.Write(new byte[padding]);
                }

                bw.Write(this.faces.Count);

                for (int i = 0; i < this.faces.Count; i++)
                {
                    bw.Write((short)this.faces[i].MaterialID);
                    bw.Write((short)0);
                    bw.Write(this.faces[i].Verts[0]);
                    bw.Write(this.faces[i].Verts[1]);
                    bw.Write(this.faces[i].Verts[2]);
                }

                bw.Write(this.verts.Count);

                for (int i = 0; i < this.verts.Count; i++)
                {
                    bw.Write(this.verts[i].Position.X);
                    bw.Write(this.verts[i].Position.Y);
                    bw.Write(this.verts[i].Position.Z);

                    bw.Write(this.verts[i].Normal.X);
                    bw.Write(this.verts[i].Normal.Y);
                    bw.Write(this.verts[i].Normal.Z);

                    bw.Write(this.verts[i].UV.X);
                    bw.Write(this.verts[i].UV.Y);

                    bw.Write(this.verts[i].UV2.X);
                    bw.Write(this.verts[i].UV2.Y);

                    bw.Write(this.verts[i].Colour.R);
                    bw.Write(this.verts[i].Colour.G);
                    bw.Write(this.verts[i].Colour.B);
                    bw.Write(this.verts[i].Colour.A);
                }

                bw.Write((short)this.meshes.Count);

                for (int i = 0; i < this.meshes.Count; i++)
                {
                    var mesh = this.meshes[i];

                    bw.Write(mesh.Extents.Centre.X);
                    bw.Write(mesh.Extents.Centre.Y);
                    bw.Write(mesh.Extents.Centre.Z);
                    bw.Write(mesh.Extents.Radius);
                    bw.Write(mesh.Extents.Min.X);
                    bw.Write(mesh.Extents.Min.Y);
                    bw.Write(mesh.Extents.Min.Z);
                    bw.Write(mesh.Extents.Max.X);
                    bw.Write(mesh.Extents.Max.Y);
                    bw.Write(mesh.Extents.Max.Z);

                    // TriangleStrips
                    bw.Write(mesh.StripOffset);
                    bw.Write(mesh.StripVertCount);
                    bw.Write(mesh.StripList.Count);

                    for (int j = 0; j < mesh.StripList.Count; j++)
                    {
                        bw.Write((uint)mesh.StripList[j].Index | (mesh.StripList[j].Degenerate ? 0x80000000 : 0x0));
                    }

                    bw.Write(mesh.TriListOffset);
                    bw.Write(mesh.TriListVertCount);
                    bw.Write(mesh.TriList.Count);

                    for (int j = 0; j < mesh.TriList.Count; j++)
                    {
                        bw.Write(mesh.TriList[j].Index);
                    }
                }

                if (this.flags.HasFlag(Flags.USERData))
                {
                    bw.Write(0);

                    for (int i = 0; i < this.verts.Count; i++)
                    {
                        bw.Write(this.verts[i].Position.X);
                        bw.Write(this.verts[i].Position.Y);
                        bw.Write(this.verts[i].Position.Z);
                        bw.Write(1);
                    }

                    for (int i = 0; i < this.faces.Count; i++)
                    {
                        var v12 = this.verts[this.faces[i].Verts[1]].Normal - this.verts[this.faces[i].Verts[0]].Normal;
                        var v13 = this.verts[this.faces[i].Verts[2]].Normal - this.verts[this.faces[i].Verts[0]].Normal;
                        var n = Vector3.Cross(v12, v13).Normalised;
                        var d = Vector3.Dot(n, this.verts[this.faces[i].Verts[0]].Normal);

                        bw.Write(d);
                        bw.Write(n.X);
                        bw.Write(n.Y);
                        bw.Write(n.Z);
                        bw.Write(this.verts[this.faces[i].Verts[0]].Normal.X);
                        bw.Write(this.verts[this.faces[i].Verts[0]].Normal.Y);
                        bw.Write(this.verts[this.faces[i].Verts[0]].Normal.Z);
                        bw.Write(this.verts[this.faces[i].Verts[1]].Normal.X);
                        bw.Write(this.verts[this.faces[i].Verts[1]].Normal.Y);
                        bw.Write(this.verts[this.faces[i].Verts[1]].Normal.Z);
                        bw.Write(this.verts[this.faces[i].Verts[2]].Normal.X);
                        bw.Write(this.verts[this.faces[i].Verts[2]].Normal.Y);
                        bw.Write(this.verts[this.faces[i].Verts[2]].Normal.Z);
                        bw.Write(this.faces[i].MaterialID);
                        bw.Write(0);
                        bw.Write(this.faces[i].Verts[0]);
                        bw.Write(this.faces[i].Verts[1]);
                        bw.Write(this.faces[i].Verts[2]);
                        bw.Write(this.verts[this.faces[i].Verts[0]].Colour.R); bw.Write(this.verts[this.faces[i].Verts[0]].Colour.G); bw.Write(this.verts[this.faces[i].Verts[0]].Colour.B); bw.Write(this.verts[this.faces[i].Verts[0]].Colour.A);
                        bw.Write(this.verts[this.faces[i].Verts[1]].Colour.R); bw.Write(this.verts[this.faces[i].Verts[1]].Colour.G); bw.Write(this.verts[this.faces[i].Verts[1]].Colour.B); bw.Write(this.verts[this.faces[i].Verts[1]].Colour.A);
                        bw.Write(this.verts[this.faces[i].Verts[2]].Colour.R); bw.Write(this.verts[this.faces[i].Verts[2]].Colour.G); bw.Write(this.verts[this.faces[i].Verts[2]].Colour.B); bw.Write(this.verts[this.faces[i].Verts[2]].Colour.A);
                        bw.Write(this.verts[this.faces[i].Verts[0]].UV.X);
                        bw.Write(this.verts[this.faces[i].Verts[0]].UV.Y);
                        bw.Write(this.verts[this.faces[i].Verts[0]].UV2.X);
                        bw.Write(this.verts[this.faces[i].Verts[0]].UV2.Y);
                        bw.Write(this.verts[this.faces[i].Verts[1]].UV.X);
                        bw.Write(this.verts[this.faces[i].Verts[1]].UV.Y);
                        bw.Write(this.verts[this.faces[i].Verts[1]].UV2.X);
                        bw.Write(this.verts[this.faces[i].Verts[1]].UV2.Y);
                        bw.Write(this.verts[this.faces[i].Verts[2]].UV.X);
                        bw.Write(this.verts[this.faces[i].Verts[2]].UV.Y);
                        bw.Write(this.verts[this.faces[i].Verts[2]].UV2.X);
                        bw.Write(this.verts[this.faces[i].Verts[2]].UV2.Y);
                        bw.Write((byte)0);
                        bw.Write(0);
                    }

                    for (int i = 0; i < this.faces.Count; i++) { bw.Write(i); }

                    bw.Write(this.verts.Count);

                    for (int i = 0; i < this.verts.Count; i++) { bw.Write(i); }

                    if (this.flags.HasFlag(Flags.USERSkinData))
                    {

                    }
                }
            }

            using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Open)))
            {
                bw.Seek(24, SeekOrigin.Begin);
                bw.Write((int)(bw.BaseStream.Length - 28));
            }
        }

        public void CalculateExtents()
        {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < this.verts.Count; i++)
            {
                if (this.verts[i].Position.X < min.X) { min.X = this.verts[i].Position.X; }
                if (this.verts[i].Position.Y < min.Y) { min.Y = this.verts[i].Position.Y; }
                if (this.verts[i].Position.Z < min.Z) { min.Z = this.verts[i].Position.Z; }

                if (this.verts[i].Position.X > max.X) { max.X = this.verts[i].Position.X; }
                if (this.verts[i].Position.Y > max.Y) { max.Y = this.verts[i].Position.Y; }
                if (this.verts[i].Position.Z > max.Z) { max.Z = this.verts[i].Position.Z; }
            }

            this.extents.Min = min;
            this.extents.Max = max;

            this.extents.Radius = (Single)Math.Max(
                        Math.Abs(Math.Sqrt(Math.Pow(min.X, 2) + Math.Pow(min.Y, 2) + Math.Pow(min.Z, 2))),
                        Math.Abs(Math.Sqrt(Math.Pow(max.X, 2) + Math.Pow(max.Y, 2) + Math.Pow(max.Z, 2)))
                      );
        }

        private Dictionary<Vector3, List<int>> generateConsolidata()
        {
            var flattened = new Dictionary<Vector3, List<int>>();

            for (int i = 0; i < this.verts.Count; i++)
            {
                var vert = this.verts[i];

                if (!flattened.ContainsKey(vert.Position))
                {
                    flattened[vert.Position] = new List<int>() { i };
                }
                else
                {
                    flattened[vert.Position].Add(i);
                }
            }

            return flattened;
        }
    }

    public class MDLFace
    {
        int materialID;
        int flags;
        int[] verts = new int[3];

        public int MaterialID { get { return materialID; } }
        public int Flags { get { return flags; } }
        public int[] Verts { get { return verts; } }

        public MDLFace(int MaterialID, int Flags, int A, int B, int C)
        {
            this.materialID = MaterialID;
            this.flags = Flags;
            this.verts[0] = A;
            this.verts[1] = B;
            this.verts[2] = C;
        }

        public override string ToString()
        {
            return "{ Face: {A:" + verts[0] + " B:" + verts[1] + " C:" + verts[2] + "} Material: " + materialID + " Flags: " + flags + " }";
        }
    }

    public class MDLExtents
    {
        Single radius;
        Vector3 min;
        Vector3 max;

        public Vector3 Min
        {
            get { return min; }
            set { min = value; }
        }

        public Vector3 Max
        {
            get { return max; }
            set { max = value; }
        }

        public Single Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public Vector3 Centre { get { return (min + max) / 2.0f; } }

        public override string ToString()
        {
            return "{ Min: " + min.ToString() + ", Max: " + max.ToString() + ", Centre: " + Centre.ToString() + ", Radius: " + radius + " }";
        }
    }

    public class MDLVertex
    {
        Vector3 position;
        Vector3 normal;
        Vector2 uv;
        Vector2 uv2;
        Color colour;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        public Vector2 UV
        {
            get { return uv; }
            set { uv = value; }
        }

        public Vector2 UV2
        {
            get { return uv2; }
            set { uv2 = value; }
        }

        public Color Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        public MDLVertex(Single X, Single Y, Single Z, Single NX, Single NY, Single NZ, Single U, Single V, Single U2, Single V2, byte R, byte G, byte B, byte Alpha)
        {
            position = new Vector3(X, Y, Z);
            normal = new Vector3(NX, NY, NZ);
            uv = new Vector2(U, V);
            uv2 = new Vector2(U2, V2);
            colour = Color.FromArgb(Alpha, R, G, B);
        }

        public override string ToString()
        {
            return "{ Position: {X:" + Position.X + " Y:" + Position.Y + " Z:" + Position.Z + "} Normal: {X:" + Normal.X + " Y:" + Normal.Y + " Z:" + Normal.Z + "} UV: {U:" + UV.X + " V:" + UV.Y + "} UV2: {A:" + uv2.X + " B:" + uv2.Y + "} { UV Length:" + UV.Length + " } }";
        }
    }

    [DebuggerDisplay("Index {Index} Degenerate {Degenerate}")]
    public class MDLPoint
    {
        int index;
        bool bDegenerate;

        public int Index { get { return index; } }
        public bool Degenerate { get { return bDegenerate; } }

        public MDLPoint(int Index, bool IsDegenerate = false)
        {
            index = Index;
            bDegenerate = IsDegenerate;
        }
    }

    public class MDLBone
    {
        string name;
        Vector3 minExtents;
        Vector3 maxExtents;
        Vector3 offset;
        byte parent;
        byte child;
        byte sibling;

        Vector4 rotation; // Actually a quat
        Vector3 position;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Vector3 MinExtents
        {
            get { return minExtents; }
            set { minExtents = value; }
        }

        public Vector3 MaxExtents
        {
            get { return maxExtents; }
            set { maxExtents = value; }
        }

        public Vector3 Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public byte Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public byte Child
        {
            get { return child; }
            set { child = value; }
        }

        public byte Sibling
        {
            get { return sibling; }
            set { sibling = value; }
        }

        public Vector4 Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
    }

    public class MDLPrepSkinWeightLookup
    {
        int count;
        int index;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }
    }

    public class MDLPrepSkinWeight
    {
        int boneIndex;
        float weight;

        public int BoneIndex
        {
            get { return boneIndex; }
            set { boneIndex = value; }
        }

        public float Weight
        {
            get { return weight; }
            set { weight = value; }
        }
    }

    public class MDLUserVertexEntry
    {
        int count;
        Vector3 position;

        public int Count { get { return count; } }
        public Vector3 Position { get { return position; } }

        public MDLUserVertexEntry(Single x, Single y, Single z, int n)
        {
            position = new Vector3(x, y, z);
            count = n;
        }
    }

    public class MDLUserFaceEntry
    {
        Vector4 plane;
        Vector3 norm1;
        Vector3 norm2;
        Vector3 norm3;
        int materialID;
        int smoothingGroup;
        int vertexID1;
        int vertexID2;
        int vertexID3;
        Color colour1;
        Color colour2;
        Color colour3;
        Vector2 uv1;
        Vector2 uv21;
        Vector2 uv2;
        Vector2 uv22;
        Vector2 uv3;
        Vector2 uv23;
        byte flags;
        int applicationFlags;

        public MDLUserFaceEntry(Single d, Single a, Single b, Single c, Vector3 n1, Vector3 n2, Vector3 n3, int materialID, int smoothingGroup, int v1, int v2, int v3, byte r1, byte g1, byte b1, byte a1, byte r2, byte g2, byte b2, byte a2, byte r3, byte g3, byte b3, byte a3, Vector2 uv1, Vector2 uv21, Vector2 uv2, Vector2 uv22, Vector2 uv3, Vector2 uv23, byte flags, int applicationFlags)
        {
            plane = new Vector4(d, a, b, c);
            norm1 = n1;
            norm2 = n2;
            norm3 = n3;
            this.materialID = materialID;
            this.smoothingGroup = smoothingGroup;
            vertexID1 = v1;
            vertexID2 = v2;
            vertexID3 = v3;
            colour1 = Color.FromArgb(a1, r1, g1, b1);
            colour2 = Color.FromArgb(a2, r2, g2, b2);
            colour3 = Color.FromArgb(a3, r3, g3, b3);
            this.uv1 = uv1;
            this.uv21 = uv21;
            this.uv2 = uv2;
            this.uv22 = uv22;
            this.uv3 = uv3;
            this.uv23 = uv23;
            this.flags = flags;
            this.applicationFlags = applicationFlags;
        }
    }

    public class MDLMaterialGroup
    {
        int index;
        string name;
        int stripOffset;
        int stripVertCount;
        int triOffset;
        int triVertCount;
        List<MDLPoint> stripList;
        List<MDLPoint> triList;
        MDLExtents extents;

        public int Index { get { return index; } }
        public string Name { get { return name; } }
        public int StripOffset { get { return stripOffset; } set { stripOffset = value; } }
        public int StripVertCount { get { return stripVertCount; } set { stripVertCount = value; } }
        public int TriListOffset { get { return triOffset; } set { triOffset = value; } }
        public int TriListVertCount { get { return triVertCount; } set { triVertCount = value; } }
        public List<MDLPoint> StripList { get { return stripList; } set { stripList = value; } }
        public List<MDLPoint> TriList { get { return triList; } set { triList = value; } }
        public MDLExtents Extents { get { return extents; } set { extents = value; } }

        public MDLMaterialGroup(int Index, string Name)
        {
            index = Index;
            name = Name;
            stripList = new List<MDLPoint>();
            triList = new List<MDLPoint>();
            extents = new MDLExtents();
        }

        public void CalculateExtents(List<MDLVertex> Vertices)
        {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var point in stripList)
            {
                min.X = Math.Min(Vertices[point.Index + stripOffset].Position.X, min.X);
                min.Y = Math.Min(Vertices[point.Index + stripOffset].Position.Y, min.Y);
                min.Z = Math.Min(Vertices[point.Index + stripOffset].Position.Z, min.Z);

                max.X = Math.Max(Vertices[point.Index + stripOffset].Position.X, max.X);
                max.Y = Math.Max(Vertices[point.Index + stripOffset].Position.Y, max.Y);
                max.Z = Math.Max(Vertices[point.Index + stripOffset].Position.Z, max.Z);
            }

            foreach (var point in triList)
            {
                min.X = Math.Min(Vertices[point.Index + triOffset].Position.X, min.X);
                min.Y = Math.Min(Vertices[point.Index + triOffset].Position.Y, min.Y);
                min.Z = Math.Min(Vertices[point.Index + triOffset].Position.Z, min.Z);

                max.X = Math.Max(Vertices[point.Index + triOffset].Position.X, max.X);
                max.Y = Math.Max(Vertices[point.Index + triOffset].Position.Y, max.Y);
                max.Z = Math.Max(Vertices[point.Index + triOffset].Position.Z, max.Z);
            }

            this.extents.Min = min;
            this.extents.Max = max;

            this.extents.Radius = (Single)Math.Max(
                                    Math.Abs(Math.Sqrt(Math.Pow(min.X, 2) + Math.Pow(min.Y, 2) + Math.Pow(min.Z, 2))),
                                    Math.Abs(Math.Sqrt(Math.Pow(max.X, 2) + Math.Pow(max.Y, 2) + Math.Pow(max.Z, 2)))
                                  );
        }
    }
}
