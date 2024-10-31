﻿(function ($) {
    'use strict';
    var table;

    var datatableInit = function () {
        table = $('#datatable-default').DataTable({
            layout: {
                topStart: {
                    buttons: ['selectAll', 'selectNone']
                }
            },
            dom: '<"row"<"col-lg-6"l><"col-lg-6"f>><"table-responsive"t>p',
            columnDefs: [
                {
                    orderable: false,
                    className: 'select-checkbox',
                    targets: 0
                }
            ],
            select: {
                style: 'os',
                selector: 'td:first-child',
                style: 'multi'
            },
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
            deficienciaId: $('#ddlDeficiencia').val(),
            etniaId: $('#ddlEtnia').val(),
            sexoId: $('#ddlSexo').val()
        };
    }

    function handleBatchPrint() {
        const selectedIds = getSelectedIds();
        const hasFilters = hasSelectedFilters();

        if (!hasFilters && selectedIds.length === 0) {
            new PNotify({
                title: 'Atenção',
                text: 'Por favor, selecione alguns alunos ou aplique filtros para impressão em lote.',
                type: 'warning'
            });
            return false;
        }

        const filters = getSelectedFilters();
        let queryString = Object.entries(filters)
            .filter(([_, value]) => value !== '')
            .map(([key, value]) => `${key}=${encodeURIComponent(value)}`)
            .join('&');

        if (selectedIds.length > 0) {
            queryString += (queryString ? '&' : '') + 'ids=' + selectedIds.join(',');
        }

        window.open(`/Aluno/ImprimirCarteirinhasLote?${queryString}`, '_blank');
    }

    $(function () {
        datatableInit();

        $('#vPesquisarAluno button[type="submit"]').removeAttr('onclick');

        $('#btnImprimirLote').on('click', function (e) {
            e.preventDefault();
            handleBatchPrint();
        });
    });

    window.tableUtils = {
        getSelectedIds: getSelectedIds,
        handleBatchPrint: handleBatchPrint
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
        showNotification();
    });

}).apply(this, [jQuery]);