(function () {
    window.kentico.pageBuilder.registerInlineEditor("text-editor", {
        init: function (options) {
            var editor = options.editor;

            editor.addEventListener("input", function () {
                if (!this.textContent) {
                    // Clear the element when text content is empty because Firefox always
                    // keeps a <br> element in the contenteditable even when it's empty
                    // which prevents the css placeholder text from appearing.
                    this.innerHTML = "";
                }

                var event = new CustomEvent("updateProperty", {
                    detail: {
                        name: options.propertyName,
                        value: this.textContent,
                        refreshMarkup: false
                    }
                });

                editor.dispatchEvent(event);
            });
        },
    });
})();
