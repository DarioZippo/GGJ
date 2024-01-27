mergeInto(LibraryManager.library, {

  Hello: function () {
    window.alert("Hello, world!");
  },
  
	IsMobileWeb: function() {
		return Module.SystemInfo.mobile;
	},
});