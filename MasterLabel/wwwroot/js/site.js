/*Search btn */
$(document).ready(function () {
    $('#search').click(function () {
        const serialNumber = $('#serial-Number').val();
        const today = new Date();

        if (!serialNumber) {
            alert('Por favor ingresa un número de serie');
            return;
        }

        $.ajax({
            url: '/Home/GetReportData',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ serialNumber: serialNumber }),
            success: function (response) {
                if (response.error) {
                    alert('Error: ' + response.error);
                    return;
                }

                try {
                    const data = typeof response === 'string' ? JSON.parse(response) : response;

                    if (data.data && data.data.length > 0) {
                        const record = data.data[0];

                        $('#job').val(record[4] || '');
                        $('#item').val(record[5] || '');
                        $('#description').val(record[7] || '');
                        $('#order-Number').val(record[8] || '');
                        $('#ordline').val(record[9] || '');
                        $('#lpn').val(record[10] || '');
                        $('#tag-Number').val(record[11] || '');
                        $('#ship-Code').val(record[12] || '');
                        $('#serial-Number').val(record[6] || '');
                        $('#subinv').val(record[13] || '');
                        $('#irno').val(record[1] || '');
                        $('#address').val(record[14] || '');
                    } else {
                        alert('No se encontraron datos para el número de serie proporcionado');
                    }
                } catch (parseError) {
                    alert('Error al procesar los datos recibidos');
                }

                document.getElementById("lpn").value = document.getElementById("serial-Number").value;
                document.getElementById("irno").value = "";

                const day = String(today.getDate()).padStart(2, '0');
                const month = String(today.getMonth() + 1).padStart(2, '0');
                const year = today.getFullYear();

                document.getElementById('date').value = `${year}-${month}-${day}`;
            },
            error: function (xhr, status, error) {
                let errorMessage = 'Error al obtener los datos. Por favor, intente nuevamente.';
                alert(errorMessage);
            }
        });
    });

    // btn Confirm
    $('#confirm').click(function () {
        const labelData = {
            SerialNumber: $('#serial-Number').val(),
            Job: $('#job').val(),
            Item: $('#item').val(),
            Description: $('#description').val(),
            OrderNumber: $('#order-Number').val(),
            OrderLine: $('#ordline').val(),
            LPN: $('#lpn').val(),
            TagNumber: $('#tag-Number').val(),
            ShipCode: $('#ship-Code').val(),
            IRNO: $('#irno').val(),
            Subinv: $('#subinv').val(),
            Date: $('#date').val(),
            Address: $('#address').val()
        };

        $.ajax({
            url: '/Home/SaveLabelData',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(labelData),
            success: function (response) {
                if (response.success) {
                    alert('Datos guardados correctamente');
                } else {
                    alert('Error al guardar los datos: ' + response.message);
                }
            },
            error: function (xhr, status, error) {
                alert('Error al guardar los datos: ' + error);
            }
        });
    });

    $('#serial-Number').keypress(function (e) {
        if (e.which == 13) {
            $('#search').click();
        }
    });
});

/*
SLPU241206658
*/