MONOXBUILD=/Library/Frameworks/Mono.framework/Commands/xbuild
VERSION=5.6

all: OneDriveSDK.Android.dll # Microsoft.Live.Android.dll 


LiveSDK.Android/Jars/LiveSDK.aar:
	# check out
	git clone https://github.com/liveservices/LiveSDK-for-Android.git external/LiveSDK-for-Android

	# build
	(cd external/LiveSDK-for-Android && chmod +x gradlew && ./gradlew wrapper && ./gradlew build)

	# copy jar
	mkdir -p LiveSDK.Android/Jars/
	cp external/LiveSDK-for-Android/src/build/outputs/aar/src-release.aar LiveSDK.Android/Jars/LiveSDK.aar

Microsoft.Live.Android.dll: LiveSDK.Android/Jars/LiveSDK.aar
	$(MONOXBUILD) /p:Configuration=Release LiveSDK.Android/LiveSDK.Android.csproj
	cp LiveSDK.Android/bin/Release/Microsoft.Live.Android.dll Microsoft.Live.Android.dll


OneDriveSDK.Android/Jars/OneDriveSDK.aar:
	# check out
	###git clone https://github.com/OneDrive/onedrive-picker-android.git external/OneDrive-Picker-for-Android

	# build
	(cd external/OneDrive-Picker-for-Android && chmod +x gradlew && ./gradlew wrapper && ./gradlew build)

	# copy jar
	mkdir -p OneDriveSDK.Android/Jars/
	cp external/OneDrive-Picker-for-Android/OneDriveSDK/build/outputs/aar/OneDriveSDK-release.aar OneDriveSDK.Android/Jars/OneDriveSDK.aar


OneDriveSDK.Android.dll: OneDriveSDK.Android/Jars/OneDriveSDK.aar
	$(MONOXBUILD) /p:Configuration=Release OneDriveSDK.Android/OneDriveSDK.Android.csproj
	cp OneDriveSDK.Android/bin/Release/OneDriveSDK.Android.dll OneDriveSDK.Android.dll
	

clean:
	-rm -rf bin/ obj/ *.sln *.userprefs *.dll *.zip 
	-rm -rf LiveSDK.Android/Jars/LiveSDK.aar external/LiveSDK-for-Android/ 
	-rm -rf OneDriveSDK.Android/Jars/OneDriveSDK.aar external/OneDrive-Picker-for-Android/ 
