document.addEventListener('DOMContentLoaded', function () {
    var tipoTicket = document.getElementById('tipoTicket');
    var camposHardware = document.getElementById('camposHardware');
    var camposSoftware = document.getElementById('camposSoftware');
    var botao = document.getElementById('botao');
    
    tipoTicket.addEventListener('change', function () {
        var selectedTipo = tipoTicket.value;
        botao.style.display = 'block';
        if (selectedTipo === 'Hardware') {
            camposHardware.style.display = 'block';
            camposSoftware.style.display = 'none';
        } else if (selectedTipo === 'Software') {
            camposHardware.style.display = 'none';
            camposSoftware.style.display = 'block';
        } else {
            camposHardware.style.display = 'none';
            camposSoftware.style.display = 'none';
            botao.style.display = 'none';
        }
    });
});

// Evento de mudança na dropdown tipoTicket
document.getElementById('tipoTicketFiltro').addEventListener('change', function () {
    var selectedOption = this.options[this.selectedIndex];
    var url = selectedOption.getAttribute('data-url');

    // Redireciona para o URL associado à opção selecionada
    if (url) {
        window.location.href = url;
    }
});


function inicializarSelect(tipoEscolhido) {
    var selectElement = document.getElementById('tipoTicketFiltro');

    for (var i = 0; i < selectElement.options.length; i++) {
        if (selectElement.options[i].value === tipoEscolhido) {
            selectElement.selectedIndex = i;
            break;
        }
    }
}

document.addEventListener("DOMContentLoaded", function () {
    var tipoEscolhido = document.getElementById('tipoEscolhido').innerText;
    inicializarSelect(tipoEscolhido);
});