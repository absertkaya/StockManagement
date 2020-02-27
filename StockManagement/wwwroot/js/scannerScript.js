window.JsFunctions = {
    scanner: function () {
        let selectedDeviceId;
        const codeReader = new ZXing.BrowserBarcodeReader()

        function resetVideo() {
            codeReader.decodeOnceFromVideoDevice(selectedDeviceId, 'video').then((result) => {
                console.log(result)
                document.getElementById('result').value = result.text
                document.getElementById('result').dispatchEvent(new Event("change"))
            }).catch((err) => {
                console.error(err)
            })
        }

        codeReader.getVideoInputDevices()
            .then((videoInputDevices) => {
                const sourceSelect = document.getElementById('sourceSelect')
                selectedDeviceId = videoInputDevices[1] ? videoInputDevices[1].deviceId : videoInputDevices[0].deviceId

                resetVideo()
                if (videoInputDevices.length > 1) {
                    videoInputDevices.forEach((element) => {
                        const sourceOption = document.createElement('option')
                        sourceOption.text = element.label
                        sourceOption.value = element.deviceId
                        sourceSelect.appendChild(sourceOption)
                        sourceSelect.value = selectedDeviceId
                    })

                    sourceSelect.onchange = () => {
                        selectedDeviceId = sourceSelect.value;
                        resetVideo()
                    }

                    const sourceSelectPanel = document.getElementById('sourceSelectPanel')
                    sourceSelectPanel.style.display = 'block'
                }

                document.getElementById('resetButton').addEventListener('click', () => {
                    document.getElementById('result').value = '';
                    codeReader.reset();
                    resetVideo()
                })

            })
            .catch((err) => {
                console.error(err)
            })
    }
}