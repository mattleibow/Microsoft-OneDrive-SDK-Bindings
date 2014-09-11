all: android

android:
	cd  binding/ && make

clean:
	cd  binding/ && make clean
