(function () {
  window.kentico.pageBuilder.registerInlineEditor("image-uploader-editor", {
    init: function (options) {
      var editor = options.editor;
      var zone = editor.querySelector(".uploader");
      var clickable = editor.querySelector(".dz-clickable");
      var isMediaFile = editor.getAttribute("data-file-type") === "media";

      if (isMediaFile) {
        var dialogLink = editor.querySelector(".dialog-link");
        dialogLink.addEventListener("click", function () {
          var dialogOptions = {
            libraryName: "Graphics",
            maxFilesLimit: 1,
            allowedExtensions: ".gif;.png;.jpg;.jpeg",
            selectedValues: options.propertyValue,
            applyCallback: function (files) {
              if (files !== null && files.length > 0) {
                var newFile = files[0];
                  if (options.propertyValue && options.propertyValue.length && newFile.fileGuid === options.propertyValue[0].fileGuid) {
                  return {
                    closeDialog: true
                  };
                }
                
                var event = new CustomEvent("updateProperty",
                  {
                    detail: {
                      value: [{ fileGuid: newFile.fileGuid }],
                      name: options.propertyName
                    }
                  });

                editor.dispatchEvent(event);
              }

              return {
                closeDialog: true
              };
            }
          };
          window.kentico.modalDialog.mediaFilesSelector.open(dialogOptions);
        });
      }

      var dropZone = new Dropzone(zone,
        {
          acceptedFiles: ".gif,.png,.jpg,.jpeg",
          maxFiles: 1,
          url: editor.getAttribute("data-url"),
          createImageThumbnails: false,
          clickable: clickable,
          dictInvalidFileType: options.localizationService.getString(
            "You can't upload files of this type.")
        });

      dropZone.on("success",
        function (e) {
          var content = JSON.parse(e.xhr.response);

          var event = new CustomEvent("updateProperty",
            {
              detail: {
                value: isMediaFile ? [{ fileGuid: content.guid }] : content.guid,
                name: options.propertyName
              }
            });

          editor.dispatchEvent(event);
        });
    },

    destroy: function (options) {
      var dropZone = options.editor.querySelector(".uploader").dropzone;
      if (dropZone) {
        dropZone.destroy();
      }
    }
  });
})();
