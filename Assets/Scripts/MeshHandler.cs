using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHandler{
    
    List<Vector3> vertices;
    List<Vector2> uvs;
    List<Vector3> normals;
    List<int> triangles;

	Vector3 meshOffset = Vector3.zero;

	void InitializeDataLists(){
		vertices = new List<Vector3>();
		uvs = new List<Vector2>();
		normals = new List<Vector3>();
		triangles = new List<int>();
	}

	//Used to build a seven vertex hexagon
	public void AssignHexMeshData(){
		InitializeDataLists();

		float hubScale = GlobalData.hubSectionScale;

		//Assigns hexagon data specifically
        for (int i = 0; i < HexData.hexCorners.Length; i++){
            vertices.Add(HexData.hexCorners[i]*hubScale+meshOffset);
            normals.Add(Vector3.up);
            uvs.Add(HexData.uvs[i]);
        }
		//Assigns hexagon triangles specifically
		for (int i = 0; i < HexData.hexCorners.Length - 1; i++){
			triangles.Add(0);
			triangles.Add(i + 1);
			triangles.Add((i >= 5) ? 1 : i + 2);
		}
    }

	//Is almost identical to section mesh, just uses a different scaling pattern
	public void AssignHullMeshData(int sectionOrder, bool isInnerHull){
		InitializeDataLists();

		int nextSection = (sectionOrder == 6)?1:sectionOrder + 1;
		float hubScale = GlobalData.hubSectionScale;
		float sectionScale = hubScale + GlobalData.cellSectionScale;
		float hullScale = sectionScale + GlobalData.hullSectionScale;
		float outerHullScale = sectionScale + GlobalData.hullSectionScale * 2f;

		//These are named wrong, but are doing the correct calculation
		Vector3 sectionVertexOne = HexData.hexCorners[sectionOrder]*sectionScale;
		Vector3 sectionVertexTwo = HexData.hexCorners[nextSection]*sectionScale;
		Vector3 hullVertexOne;
		Vector3 hullVertexTwo;
		if(isInnerHull){
			hullVertexOne = HexData.hexCorners[sectionOrder]*hullScale;
			hullVertexTwo = HexData.hexCorners[nextSection]*hullScale;
		}
		else{
			hullVertexOne = HexData.hexCorners[sectionOrder]*(outerHullScale);
			hullVertexTwo = HexData.hexCorners[nextSection]*(outerHullScale);
		}

		vertices.Add(sectionVertexOne+meshOffset);
		vertices.Add(hullVertexOne+meshOffset);
		vertices.Add(hullVertexTwo+meshOffset); //Comes before second hub vertex for triangle sequencing
		vertices.Add(sectionVertexTwo+meshOffset);

		for (int i = 0; i < 2; i++) {
			triangles.Add(0);
			triangles.Add(i+1);
			triangles.Add(i+2);
		}

		for (int i = 0; i < 4; i++) {
			normals.Add(Vector2.up);
			uvs.Add(Vector2.zero);
			//Can maybe make uvs work by ratioing them using hub and section scales
		}
	}

	//Used to build a four vertex hex section
	//SECTIONORDER CANNOT BE ZERO; THAT VERTEX WILL NOT WORK
	//distFromHexCenter allows for modding size of the section
	public void AssignSectionMeshData(int sectionOrder){
		InitializeDataLists();

		int nextSection = (sectionOrder == 6)?1:sectionOrder + 1;
		float hubScale = GlobalData.hubSectionScale;
		float sectionScale = hubScale + GlobalData.cellSectionScale;
		Vector3 hubVertexOne = HexData.hexCorners[sectionOrder]*hubScale;
		Vector3 sectionVertexOne = HexData.hexCorners[sectionOrder]*sectionScale;
		Vector3 hubVertexTwo = HexData.hexCorners[nextSection]*hubScale;
		Vector3 sectionVertexTwo = HexData.hexCorners[nextSection]*sectionScale;

		vertices.Add(hubVertexOne+meshOffset);
		vertices.Add(sectionVertexOne+meshOffset);
		vertices.Add(sectionVertexTwo+meshOffset); //Comes before second hub vertex for triangle sequencing
		vertices.Add(hubVertexTwo+meshOffset);

		for (int i = 0; i < 2; i++) {
			triangles.Add(0);
			triangles.Add(i+1);
			triangles.Add(i+2);
		}

		for (int i = 0; i < 4; i++) {
			normals.Add(Vector2.up);
			uvs.Add(Vector2.zero);
			//Can maybe make uvs work by ratioing them using hub and section scales
		}
	}

    public void ReturnCompleteMesh(MeshFilter filter)
    {
        Mesh mesh = new Mesh();

        MeshFilter mF = filter;

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();

        mF.sharedMesh = mesh;        
    }

	public void SetMeshOffset(Vector3 _meshOffset){
		meshOffset = _meshOffset;
	}
}