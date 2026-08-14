mergeInto(LibraryManager.library, {
  CloseBrowserTab: function () {
    window.close();
    if (!window.closed) {
      window.open("", "_self");
      window.close();
    }
  }
});
