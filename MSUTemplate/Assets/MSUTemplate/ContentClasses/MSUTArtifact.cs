﻿using MSU;
using R2API.ScriptableObjects;
using RoR2;
using RoR2.ContentManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MSUTemplate
{
    /// <summary>
    /// <inheritdoc cref="IArtifactContentPiece"/>
    /// </summary>
    public abstract class MSUTArtifact : IArtifactContentPiece, IContentPackModifier
    {
        /// <summary>
        /// The ArtifactAssetCollection for this Artifact. Populated when the Artifact gets it's assets loaded, can be null.
        /// </summary>
        public NullableRef<ArtifactAssetCollection> assetCollection { get; private set; }
        /// <inheritdoc cref="IArtifactContentPiece.ArtifactCode"/>
        public NullableRef<ArtifactCode> artifactCode { get; protected set; }
        /// <summary>
        /// <inheritdoc cref="IContentPiece{T}.Asset"/>
        /// </summary>
        public ArtifactDef artifactDef { get; protected set; }

        NullableRef<ArtifactCode> IArtifactContentPiece.artifactCode => artifactCode;
        ArtifactDef IContentPiece<ArtifactDef>.asset => artifactDef;

        /// <summary>
        /// Method for loading an AssetRequest for this class. This will later get loaded Asynchronously.
        /// </summary>
        /// <returns>An ExampleAssetRequest</returns>
        public abstract MSUTAssetRequest LoadAssetRequest();

        public abstract void Initialize();
        public abstract bool IsAvailable(ContentPack contentPack);

        public virtual IEnumerator LoadContentAsync()
        {
            MSUTAssetRequest request = LoadAssetRequest();

            request.StartLoad();
            while (!request.isComplete)
                yield return null;

            if(request.boxedAsset is ArtifactAssetCollection collection)
            {
                assetCollection = collection;
                artifactDef = collection.artifactDef;
                artifactCode = collection.artifactCode;
            }
            else if(request.boxedAsset is ArtifactDef artifact)
            {
                artifactDef = artifact;
            }
            else
            {
                MSUTLog.Error($"Invalid AssetRequest {request.assetName} of type {request.boxedAsset.GetType()}");
            }
        }

        public abstract void OnArtifactDisabled();
        public abstract void OnArtifactEnabled();

        //If an asset collection was loaded, the asset collection will be added to your mod's ContentPack.
        public void ModifyContentPack(ContentPack contentPack)
        {
            if (assetCollection)
                contentPack.AddContentFromAssetCollection(assetCollection);
        }
    }
}