﻿using System.Linq;
using Orchard.ContentManagement;
using Orchard.Layouts.Elements;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Layouts.ViewModels;
using Orchard.MediaLibrary.Models;
using ContentItem = Orchard.Layouts.Elements.ContentItem;

namespace Orchard.Layouts.Drivers {
    public class VectorImageDriver : ElementDriver<VectorImage> {
        private readonly IContentManager _contentManager;

        public VectorImageDriver(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        protected override EditorResult OnBuildEditor(VectorImage element, ElementEditorContext context) {

            var viewModel = new VectorImageEditorViewModel {
                VectorImageId = element.MediaId.ToString(),
                Width = element.Width,
                Height = element.Height
            };
            var editor = context.ShapeFactory.EditorTemplate(TemplateName: "Elements.VectorImage", Model: viewModel);

            if (context.Updater != null) {
                context.Updater.TryUpdateModel(viewModel, context.Prefix, null, null);
                element.MediaId = ParseVectorImageId(viewModel.VectorImageId);
                element.Width = viewModel.Width;
                element.Height = viewModel.Height;
            }

            var mediaId = element.MediaId;
            viewModel.CurrentVectorImage = mediaId != null ? GetVectorImage(mediaId.Value) : default(VectorImagePart);

            return Editor(context, editor);
        }

        protected override void OnDisplaying(VectorImage element, ElementDisplayContext context) {
            var mediaId = element.MediaId;
            var vectorImage = mediaId != null ? GetVectorImage(mediaId.Value) : default(VectorImagePart);
            context.ElementShape.VectorImagePart = vectorImage;
        }

        protected VectorImagePart GetVectorImage(int id) {
            return _contentManager.Get<VectorImagePart>(id, VersionOptions.Published);
        }

        private static int? ParseVectorImageId(string vectorImageId) {
            return ContentItem.Deserialize(vectorImageId).FirstOrDefault();
        }
    }
}