MONOXBUILD=/Library/Frameworks/Mono.framework/Commands/xbuild
VERSION=5.6

OLD_VERSION=com.android.tools.build:gradle:0.9.\+
NEW_VERSION=com.android.tools.build:gradle:0.12.\+

all: LiveSDK.Android.dll

external/LiveSDK-for-Android:
	git clone https://github.com/liveservices/LiveSDK-for-Android.git external/LiveSDK-for-Android
	git --git-dir=external/LiveSDK-for-Android/.git checkout 2c2507bc0c06138987a2864fa68e33a4ac537f8c

external/LiveSDK-for-Android/src/build/bundles/release/classes.jar:	external/LiveSDK-for-Android
	# use the latestandroid tools (0.12)
	#	(cd external/LiveSDK-for-Android && find . -type f -name 'build.gradle' -exec perl -p -i -e 's/$(OLD_VERSION)/$(NEW_VERSION)/g' {} \;)
	# we may also need to change the build android tools and platform version for the CI
	
	(cd external/LiveSDK-for-Android && chmod +x gradlew && ./gradlew wrapper && ./gradlew build)
	(cd external/LiveSDK-for-Android/src/build/bundles/release && zip -r release.zip .)
	
PrepareJar: external/LiveSDK-for-Android/src/build/bundles/release/classes.jar
	mkdir -p LiveSDK.Android/Jars/
	cp external/LiveSDK-for-Android/src/build/bundles/release/classes.jar LiveSDK.Android/Jars/LiveSDK.jar

LiveSDK.Android.dll: PrepareJar
	$(MONOXBUILD) /p:Configuration=Release LiveSDK.Android/LiveSDK.Android.csproj
	cp LiveSDK.Android/bin/Release/LiveSDK.Android.dll LiveSDK.Android.dll

clean:
	-rm -rf bin/ obj/ *.sln *.userprefs *.dll LiveSDK.Android/Jars/LiveSDK.zip *.zip external/LiveSDK-for-Android/
