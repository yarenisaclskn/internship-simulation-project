class AircraftController {
    constructor(viewer) {
        this.viewer = viewer;
        this.aircraftEntity = null;
        this.currentRoute = [];
        this.currentWaypointIndex = 0;
        this.isMoving = false;
    }

    // İki nokta arasındaki heading (yönelim) hesaplama
    calculateHeading(startLon, startLat, endLon, endLat) {
        var lat1 = Cesium.Math.toRadians(startLat);
        var lat2 = Cesium.Math.toRadians(endLat);
        var deltaLon = Cesium.Math.toRadians(endLon - startLon);

        var y = Math.sin(deltaLon) * Math.cos(lat2);
        var x = Math.cos(lat1) * Math.sin(lat2) - Math.sin(lat1) * Math.cos(lat2) * Math.cos(deltaLon);

        var bearing = Math.atan2(y, x);
        bearing = Cesium.Math.toDegrees(bearing);
        return (bearing + 360) % 360;
    }

    // İki nokta arasındaki mesafe hesaplama (km)
    calculateDistance(lat1, lon1, lat2, lon2) {
        var R = 6371; // Dünya yarıçapı km
        var dLat = Cesium.Math.toRadians(lat2 - lat1);
        var dLon = Cesium.Math.toRadians(lon2 - lon1);
        var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.cos(Cesium.Math.toRadians(lat1)) * Math.cos(Cesium.Math.toRadians(lat2)) *
            Math.sin(dLon / 2) * Math.sin(dLon / 2);
        var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        return R * c;
    }

    // Uçak entity'sini oluşturma
    initializeAircraft(modelPath = null) {
        if (this.aircraftEntity) {
            this.viewer.entities.remove(this.aircraftEntity);
        }

        var entityConfig = {
            name: 'Aircraft',
            position: Cesium.Cartesian3.fromDegrees(0, 0, 10000),
            orientation: Cesium.Transforms.headingPitchRollQuaternion(
                Cesium.Cartesian3.fromDegrees(0, 0, 10000),
                new Cesium.HeadingPitchRoll(0, 0, 0)
            )
        };

        if (modelPath) {
            entityConfig.model = {
                uri: modelPath,
                minimumPixelSize: 64,
                maximumScale: 20000,
                scale: 1.0
            };
        } else {
            entityConfig.billboard = {
                image: this.createAircraftIcon(),
                width: 32,
                height: 32,
                rotation: 0,
                alignedAxis: Cesium.Cartesian3.UNIT_Z
            };
        }

        this.aircraftEntity = this.viewer.entities.add(entityConfig);
        return this.aircraftEntity;
    }

    // Basit uçak ikonu (model yoksa)
    createAircraftIcon() {
        var canvas = document.createElement('canvas');
        canvas.width = 32;
        canvas.height = 32;
        var ctx = canvas.getContext('2d');
        ctx.fillStyle = '#ffffff';
        ctx.strokeStyle = '#000000';
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.moveTo(16, 2);
        ctx.lineTo(6, 20);
        ctx.lineTo(12, 20);
        ctx.lineTo(12, 26);
        ctx.lineTo(16, 30);
        ctx.lineTo(20, 26);
        ctx.lineTo(20, 20);
        ctx.lineTo(26, 20);
        ctx.closePath();
        ctx.fill();
        ctx.stroke();
        return canvas.toDataURL();
    }

    // Pozisyon ve yönelim ayarla
    updateAircraft(currentLon, currentLat, altitude, heading, pitch = 0, roll = 0) {
        var position = Cesium.Cartesian3.fromDegrees(currentLon, currentLat, altitude);

        // Normal HPR
        var hpr = new Cesium.HeadingPitchRoll(
            Cesium.Math.toRadians(heading), // heading derece -> rad
            Cesium.Math.toRadians(pitch),
            Cesium.Math.toRadians(roll)
        );
        var orientation = Cesium.Transforms.headingPitchRollQuaternion(position, hpr);

        // --- MODEL DÜZELTME ---
        // Eğer uçak modelin 90 derece yamuksa buradan ayarla
        var fix = Cesium.Quaternion.fromAxisAngle(
            Cesium.Cartesian3.UNIT_Z,
            Cesium.Math.toRadians(90) // burayı -90 veya 180 yaparak dene
        );
        orientation = Cesium.Quaternion.multiply(orientation, fix, new Cesium.Quaternion());

        // Güncelle
        this.aircraftEntity.position = position;
        this.aircraftEntity.orientation = orientation;

        // Billboard için yön düzeltme
        if (this.aircraftEntity.billboard) {
            this.aircraftEntity.billboard.rotation = Cesium.Math.toRadians(heading);
        }

        console.log(`Aircraft updated - Heading: ${heading.toFixed(2)}°, Position: ${currentLat.toFixed(4)}, ${currentLon.toFixed(4)}`);
    }
}
