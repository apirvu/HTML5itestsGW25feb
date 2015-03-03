/**
 * Copyright (c) 2007-2013, Kaazing Corporation. All rights reserved.
 */

// from quirksmode.org
var BrowserDetect = {
    init: function () {
        this.browser = this.searchString(this.dataBrowser) || "An unknown browser";
        this.version = this.searchVersion(navigator.userAgent)
            || this.searchVersion(navigator.appVersion)
            || "an unknown version";
        this.OS = this.searchString(this.dataOS) || "an unknown OS";
    },
    searchString: function (data) {
        for (var i=0;i<data.length;i++) {
            var dataString = data[i].string;
            var dataProp = data[i].prop;
            this.versionSearchString = data[i].versionSearch || data[i].identity;
            if (dataString) {
                if (dataString.indexOf(data[i].subString) != -1)
                    return data[i].identity;
            }
            else if (dataProp)
                return data[i].identity;
        }
    },
    searchVersion: function (dataString) {
        var index = dataString.indexOf(this.versionSearchString);
        if (index == -1) return;
        return parseFloat(dataString.substring(index+this.versionSearchString.length+1));
    },
    dataBrowser: [
        {
            string: navigator.userAgent,
            subString: "Chrome",
            identity: "Chrome"
        },
        {   string: navigator.userAgent,
            subString: "OmniWeb",
            versionSearch: "OmniWeb/",
            identity: "OmniWeb"
        },
        {
            string: navigator.vendor,
            subString: "Apple",
            identity: "Safari",
            versionSearch: "Version"
        },
        {
            prop: window.opera,
            identity: "Opera"
        },
        {
            string: navigator.vendor,
            subString: "iCab",
            identity: "iCab"
        },
        {
            string: navigator.vendor,
            subString: "KDE",
            identity: "Konqueror"
        },
        {
            string: navigator.userAgent,
            subString: "Firefox",
            identity: "Firefox"
        },
        {
            string: navigator.vendor,
            subString: "Camino",
            identity: "Camino"
        },
        {       // for newer Netscapes (6+)
            string: navigator.userAgent,
            subString: "Netscape",
            identity: "Netscape"
        },
        {
            string: navigator.userAgent,
            subString: "MSIE",
            identity: "Explorer",
            versionSearch: "MSIE"
        },
        {
            string: navigator.userAgent,
            subString: "Gecko",
            identity: "Mozilla",
            versionSearch: "rv"
        },
        {       // for older Netscapes (4-)
            string: navigator.userAgent,
            subString: "Mozilla",
            identity: "Netscape",
            versionSearch: "Mozilla"
        }
    ],
    dataOS : [
        {
            string: navigator.platform,
            subString: "Win",
            identity: "Windows"
        },
        {
            string: navigator.platform,
            subString: "Mac",
            identity: "Mac"
        },
        {
               string: navigator.userAgent,
               subString: "iPhone",
               identity: "iPhone/iPod"
        },
        {
            string: navigator.platform,
            subString: "Linux",
            identity: "Linux"
        }
    ]
  };

BrowserDetect.init();

function detectFlashVersion() {
    var version = null;

    // ie looks something like this:
    // "WIN 9,0,124,0"
    if (typeof(ActiveXObject) != "undefined") {
        try {
            ie = true;
            var swf = new ActiveXObject("ShockwaveFlash.ShockwaveFlash");
            var version_string = swf.GetVariable("$version");
            // TODO: use match (below also)
            var versionArray = version_string.split(" ")[1].split(",");
            version = [];
            for (var i=0; i < versionArray.length; i++) {
                version[i] = parseInt(versionArray[i]);
            }
        } catch (e) {
            ie = false;
        }
    }

    // On other browsers it looks like this:
    // "Shockwave Flash 9.0 r124"
    if (typeof navigator.plugins != "undefined") {
        if (typeof navigator.plugins["Shockwave Flash"] != "undefined") {
            var version_string = navigator.plugins["Shockwave Flash"].description;
            version_string = version_string.replace(/\s*r/g, ".");
            var versionArray = version_string.split(" ")[2].split(".");
            version = [];
            for (var i=0; i < versionArray.length; i++) {
                version[i] = parseInt(versionArray[i]);
            }
        }
    }

    return version;
};

function hasMinimumFlashVersion(minimumVersion) {
     var flashVersion = detectFlashVersion();

     // not installed
     if (flashVersion == null) {
             return false;
     }

     for (var i=0; i < Math.max(flashVersion.length, minimumVersion.length); i++) {
         var difference = flashVersion[i] - minimumVersion[i];
         if (difference != 0) {
                 return (difference > 0) ? true : false;
         }
     }

     // exact match
     return true;
 }

function flashSupported() {
    return hasMinimumFlashVersion([9,0,115]);
}

function javaSupported() {
    if (BrowserDetect.OS == "Mac")
      return false;

    if (BrowserDetect.browser == "Safari")
      return false;

    if (BrowserDetect.browser == "Firefox" && BrowserDetect.version <= 2.0)
      return false;

    if (BrowserDetect.browser == "Explorer" && BrowserDetect.version < 7.0)
      return false;

    if (!deployJava.versionCheck("1.6.0_0+"))
      return false;

    return true;
}

function silverlightSupported() {
    if (BrowserDetect.browser == "Chrome" && BrowserDetect.OS == "Mac")
      return false;

    if (BrowserDetect.browser == "Safari" && BrowserDetect.OS == "Windows")
      return false;

    if (BrowserDetect.OS == "Linux")
        return false;

    return true;
}
