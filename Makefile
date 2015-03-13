MONOXBUILD=/Library/Frameworks/Mono.framework/Commands/xbuild

all: onedrivesdk livesdk

onedrivesdk: binding/OneDriveSDK.Android.dll
	xamarin-component package packaging/OneDriveSDK
	cp packaging/OneDriveSDK/*.xam ./
livesdk: binding/LiveSDK.Android.dll 
	xamarin-component package packaging/LiveSDK
	cp packaging/LiveSDK/*.xam ./


binding/LiveSDK.Android/Jars/LiveSDK.aar:
	# check out
	git clone https://github.com/liveservices/LiveSDK-for-Android.git binding/external/LiveSDK-for-Android

	# build
	(cd binding/external/LiveSDK-for-Android && chmod +x gradlew && ./gradlew wrapper && ./gradlew build)

	# copy jar
	mkdir -p binding/LiveSDK.Android/Jars/
	cp binding/external/LiveSDK-for-Android/src/build/outputs/aar/src-release.aar binding/LiveSDK.Android/Jars/LiveSDK.aar

binding/LiveSDK.Android.dll: binding/LiveSDK.Android/Jars/LiveSDK.aar
	$(MONOXBUILD) /p:Configuration=Release binding/LiveSDK.Android/LiveSDK.Android.csproj
	cp binding/LiveSDK.Android/bin/Release/LiveSDK.Android.dll binding/LiveSDK.Android.dll


binding/OneDriveSDK.Android/Jars/OneDriveSDK.aar:
	# check out
	git clone https://github.com/OneDrive/onedrive-picker-android.git binding/external/OneDrive-Picker-for-Android

	# build
	(cd binding/external/OneDrive-Picker-for-Android && chmod +x gradlew && ./gradlew wrapper && ./gradlew build)

	# copy jar
	mkdir -p binding/OneDriveSDK.Android/Jars/
	cp binding/external/OneDrive-Picker-for-Android/OneDriveSDK/build/outputs/aar/OneDriveSDK-release.aar binding/OneDriveSDK.Android/Jars/OneDriveSDK.aar

binding/OneDriveSDK.Android.dll: binding/OneDriveSDK.Android/Jars/OneDriveSDK.aar
	$(MONOXBUILD) /p:Configuration=Release binding/OneDriveSDK.Android/OneDriveSDK.Android.csproj
	cp binding/OneDriveSDK.Android/bin/Release/OneDriveSDK.Android.dll binding/OneDriveSDK.Android.dll


clean:
	-rm -rf bin/ obj/ *.userprefs *.dll *.zip *.xam binding/*.dll
	-rm -rf packaging/OneDriveSDK/*.xam packaging/LiveSDK/*.xam 
	-rm -rf binding/LiveSDK.Android/Jars/LiveSDK.aar binding/external/LiveSDK-for-Android/ 
	-rm -rf binding/OneDriveSDK.Android/Jars/OneDriveSDK.aar binding/external/OneDrive-Picker-for-Android/ 
