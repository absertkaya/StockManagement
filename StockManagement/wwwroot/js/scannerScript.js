window.JsFunctions = {
    scanner: function () {
        let selectedDeviceId;
        const codeReader = new ZXing.BrowserBarcodeReader()
        console.log('ZXing code reader initialized')
        codeReader.getVideoInputDevices()
            .then((videoInputDevices) => {
                const sourceSelect = document.getElementById('sourceSelect')
                selectedDeviceId = videoInputDevices[0].deviceId
                codeReader.decodeOnceFromVideoDevice(selectedDeviceId, 'video').then((result) => {
                    console.log(result)
                    document.getElementById('result').value = result.text
                    document.getElementById('result').dispatchEvent(new Event("change"))
                }).catch((err) => {
                    console.error(err)
                })
                if (videoInputDevices.length > 1) {
                    videoInputDevices.forEach((element) => {
                        const sourceOption = document.createElement('option')
                        sourceOption.text = element.label
                        sourceOption.value = element.deviceId
                        sourceSelect.appendChild(sourceOption)
                    })

                    sourceSelect.onchange = () => {
                        selectedDeviceId = sourceSelect.value;
                    }

                    const sourceSelectPanel = document.getElementById('sourceSelectPanel')
                    sourceSelectPanel.style.display = 'block'
                }

                document.getElementById('startButton').addEventListener('click', () => {
                    codeReader.decodeOnceFromVideoDevice(selectedDeviceId, 'video').then((result) => {
                        console.log(result)
                        document.getElementById('result').value = result.text
                        document.getElementById('result').dispatchEvent(new Event("change"))
                    }).catch((err) => {
                        console.error(err)
                    })
                    console.log(`Started continous decode from camera with id ${selectedDeviceId}`)
                })

                document.getElementById('resetButton').addEventListener('click', () => {
                    document.getElementById('result').value = '';
                    codeReader.reset();
                    console.log('Reset.')
                })

            })
            .catch((err) => {
                console.error(err)
            })
    }
}