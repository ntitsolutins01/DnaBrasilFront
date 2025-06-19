(function ($) {
    'use strict';
    var table;

    var datatableInit = function () {
        table = $('#datatable-default').DataTable({
            dom: '<"row"<"col-lg-6"l><"col-lg-6"f>><"table-responsive"t>p',
            order: [[1, 'asc']],
            paging: true,
            searching: true,
            language: {
                sEmptyTable: "Nenhum registro encontrado",
                sInfo: "Mostrando de _START_ até _END_ de _TOTAL_ registros",
                sInfoEmpty: "Mostrando 0 até 0 de 0 registros",
                sInfoFiltered: "(Filtrados de _MAX_ registros)",
                sInfoPostFix: "",
                sInfoThousands: ".",
                sLengthMenu: "_MENU_ resultados por página",
                sLoadingRecords: "Carregando...",
                sProcessing: "Processando...",
                sZeroRecords: "Nenhum registro encontrado",
                sSearch: "Pesquisar: ",
                oPaginate: {
                    sNext: "Próximo →",
                    sPrevious: "← Anterior",
                    sFirst: "Primeiro",
                    sLast: "Último"
                },
                oAria: {
                    sSortAscending: ": Ordenar colunas de forma ascendente",
                    sSortDescending: ": Ordenar colunas de forma descendente"
                }
            }
        });
    };

    function getSelectedIds() {
        return table.rows('.selected').data().map(function (row) {
            return row[1]; 
        }).toArray();
    }

    function hasSelectedFilters() {
        const filters = [
            'ddlFomento',
            'ddlEstado',
            'ddlMunicipio',
            'ddlLocalidade',
            'ddlProfissional',
            'ddlDeficiencia',
            'ddlEtnia',
            'ddlSexo'
        ];

        return filters.some(filterId => $('#' + filterId).val() !== '');
    }

    function getSelectedFilters() {
        return {
            fomentoId: $('#ddlFomento').val(),
            estadoId: $('#ddlEstado').val(),
            municipioId: $('#ddlMunicipio').val(),
            localidadeId: $('#ddlLocalidade').val(),
            profissionalId: $('#ddlProfissional').val(),
            deficienciaId: $('#ddlDeficiencia').val(),
            etniaId: $('#ddlEtnia').val(),
            sexoId: $('#ddlSexo').val(),
            possuiFoto: $('#possuiFoto').prop('checked')
        };
    }

    function obterContagemAlunos(callback) {
        const filters = getSelectedFilters();
        const selectedIds = getSelectedIds();

        let queryParams = Object.entries(filters)
            .filter(([_, value]) => value !== '')
            .map(([key, value]) => `${key}=${encodeURIComponent(value)}`)
            .join('&');

        if (selectedIds.length > 0) {
            queryParams += (queryParams ? '&' : '') + 'ids=' + selectedIds.join(',');
        }

        // Adicionando um parâmetro para indicar que queremos apenas a contagem
        queryParams += (queryParams ? '&' : '') + 'apenasContagem=true';

        $.ajax({
            url: '/Aluno/ObterContagemAlunos?' + queryParams,
            type: 'GET',
            success: function (data) {
                callback(data);
            },
            error: function () {
                new PNotify({
                    title: 'Erro',
                    text: 'Não foi possível obter a contagem de alunos. Tente novamente.',
                    type: 'error'
                });
                callback({ total: 0 });
            }
        });
    }

    // Função para configurar o modal de paginação
    function configurarModalPaginacao(totalAlunos) {
        const itensPorPagina = 20;
        const totalPaginas = Math.ceil(totalAlunos / itensPorPagina);

        $('#totalCarteirinhas').text(totalAlunos);
        $('#totalPaginas').text(totalPaginas);

        // Limpar e preencher o dropdown de páginas
        const selectPagina = $('#paginaAtual');
        selectPagina.empty();

        for (let i = 1; i <= totalPaginas; i++) {
            const inicio = ((i - 1) * itensPorPagina) + 1;
            const fim = Math.min(i * itensPorPagina, totalAlunos);
            selectPagina.append(`<option value="${i}">Página ${i} (carteirinhas ${inicio} a ${fim})</option>`);
        }

        // Abrir o modal
        $('#mdPaginacaoImpressaoLote').modal('show');
    }

    // Função para executar a impressão em lote com paginação
    function executarImpressaoLote(pagina) {
        const itensPorPagina = 20;
        const filters = getSelectedFilters();
        const selectedIds = getSelectedIds();

        let queryParams = Object.entries(filters)
            .filter(([_, value]) => value !== '')
            .map(([key, value]) => `${key}=${encodeURIComponent(value)}`)
            .join('&');

        if (selectedIds.length > 0) {
            queryParams += (queryParams ? '&' : '') + 'ids=' + selectedIds.join(',');
        }

        // Adicionar parâmetros de paginação
        queryParams += (queryParams ? '&' : '') + `pagina=${pagina}&itensPorPagina=${itensPorPagina}`;

        window.open(`/Aluno/ImprimirCarteirinhasLote?${queryParams}`, '_blank');
    }

    function handleBatchPrint(e) {
        if (e) {
            e.preventDefault();
        }

        const selectedIds = getSelectedIds();
        const hasFilters = hasSelectedFilters();
        const fomentoId = $('#ddlFomento').val();

        if (!fomentoId) {
            new PNotify({
                title: 'Atenção',
                text: 'Por favor, selecione o Fomento antes de realizar a impressão em lote.',
                type: 'warning'
            });
            return;
        }

        if (!hasFilters && selectedIds.length === 0) {
            new PNotify({
                title: 'Atenção',
                text: 'Por favor, selecione alguns alunos ou aplique filtros para impressão em lote.',
                type: 'warning'
            });
            return;
        }

        // Em vez de redirecionar diretamente, obter a contagem e exibir o modal
        obterContagemAlunos(function (data) {
            if (data.total > 0) {
                configurarModalPaginacao(data.total);
            } else {
                new PNotify({
                    title: 'Atenção',
                    text: 'Nenhum aluno encontrado com os filtros especificados.',
                    type: 'warning'
                });
            }
        });
    }

    function handleA4Print(e) {
        if (e) {
            e.preventDefault();
        }

        const selectedIds = getSelectedIds();
        const hasFilters = hasSelectedFilters();
        const fomentoId = $('#ddlFomento').val();

        if (!fomentoId) {
            new PNotify({
                title: 'Atenção',
                text: 'Por favor, selecione o Fomento antes de realizar a impressão em formato A4.',
                type: 'warning'
            });
            return;
        }

        if (!hasFilters && selectedIds.length === 0) {
            new PNotify({
                title: 'Atenção',
                text: 'Por favor, selecione alguns alunos ou aplique filtros para impressão em formato A4.',
                type: 'warning'
            });
            return;
        }

        const filters = getSelectedFilters();
        let queryString = Object.entries(filters)
            .filter(([_, value]) => value !== '')
            .map(([key, value]) => `${key}=${encodeURIComponent(value)}`)
            .join('&');

        if (selectedIds.length > 0) {
            queryString += (queryString ? '&' : '') + 'ids=' + selectedIds.join(',');
        }

        window.open(`/Aluno/ImprimirCarteirinhasA4?${queryString}`, '_blank');
    }

    $(function () {
        datatableInit();

        $('#vPesquisarAluno button[type="submit"]').removeAttr('onclick');

        $('#btnConfirmarImpressaoLote').on('click', function () {
            const paginaSelecionada = $('#paginaAtual').val();
            executarImpressaoLote(paginaSelecionada);

            // Mostrar feedback que a impressão foi iniciada sem fechar o modal
            new PNotify({
                title: 'Impressão iniciada',
                text: 'A impressão da página ' + paginaSelecionada + ' foi iniciada em uma nova aba.',
                type: 'success',
                delay: 3000
            });

            // Não fechamos mais o modal: //$('#mdPaginacaoImpressaoLote').modal('hide');
        });

        $('#btnImprimirLote').on('click', function (e) {
            e.preventDefault();
            handleBatchPrint(e);
        });

        $('#btnImprimirA4').on('click', function (e) {
            e.preventDefault();
            handleA4Print(e);
        });
    });

    window.tableUtils = {
        getSelectedIds: getSelectedIds,
        handleBatchPrint: handleBatchPrint,
        handleA4Print: handleA4Print
    };
        function getUrlParameters() {
        const queryString = window.location.search;
        const urlParams = new URLSearchParams(queryString);
        return {
            notify: urlParams.get('notify'),
            message: urlParams.get('message')
        };
    }
    function showNotification() {
        const params = getUrlParameters();

        if (params.notify && params.message) {
            let notifyType;
            switch (params.notify) {
                case '1': // Warning
                    notifyType = 'warning';
                    break;
                case '2': // Success
                    notifyType = 'success';
                    break;
                case '3': // Error
                    notifyType = 'error';
                    break;
                case '4': // Info
                    notifyType = 'info';
                    break;
                default:
                    notifyType = 'info';
            }

            new PNotify({
                title: getNotificationTitle(notifyType),
                text: decodeURIComponent(params.message),
                type: notifyType,
            });

            const newUrl = window.location.pathname;
            window.history.pushState({}, '', newUrl);
        }
    }

    function getNotificationTitle(type) {
        switch (type) {
            case 'warning':
                return 'Atenção';
            case 'success':
                return 'Sucesso';
            case 'error':
                return 'Erro';
            case 'info':
                return 'Informação';
            default:
                return 'Notificação';
        }
    }

    $(document).ready(function () {
        //showNotification();
    });

}).apply(this, [jQuery]);