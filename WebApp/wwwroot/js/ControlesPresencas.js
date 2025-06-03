var vm = new Vue({
    el: "#vControlePresenca",
    data: {
        params: {
            alunos: [],
            visible: false
        },
        loading: false,
        editDto: { Id: "", Controle: "", Justificativa: "", Data: "", NomeAluno: "", MunicipioEstado: "", NomeLocalidade: "", AlunoId: "" }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            var formid = $('form')[1].id;

            if (formid === "formControlePresencas") {

                if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                    $(function () {
                        $('[data-plugin-ios-switch]').each(function () {
                            var $this = $(this);

                            $this.themePluginIOS7Switch();
                        });
                    });
                }

                var $select = $(".select2").select2({
                    allowClear: true
                });

                $(".select2").each(function () {
                    var $this = $(this),
                        opts = {};

                    var pluginOptions = $this.data('plugin-options');
                    if (pluginOptions)
                        opts = pluginOptions;

                    $this.themePluginSelect2(opts);
                });

                /*
                 * When you change the value the select via select2, it triggers
                 * a 'change' event, but the jquery validation plugin
                 * only re-validates on 'blur'*/

                $select.on('change', function () {
                    $(this).trigger('blur');
                });

                // inicia datatable aluno
                var datatableInit = function () {

                    $('#alunoDataTable').dataTable({
                        dom: '<"row"<"col-lg-6"l><"col-lg-6"f>><"table-responsive"t>p',
                        "language": {
                            "sEmptyTable": "Nenhum registro encontrado",
                            "sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
                            "sInfoEmpty": "Mostrando 0 até 0 de 0 registros",
                            "sInfoFiltered": "(Filtrados de _MAX_ registros)",
                            "sInfoPostFix": "",
                            "sInfoThousands": ".",
                            "sLengthMenu": "_MENU_ resultados por página",
                            "sLoadingRecords": "Carregando...",
                            "sProcessing": "Processando...",
                            "sZeroRecords": "Nenhum registro encontrado",
                            "sSearch": "Pesquisar: ",
                            "oPaginate": {
                                "sNext": "Próximo →" +
                                    "" +
                                    "",
                                "sPrevious": "← Anterior",
                                "sFirst": "Primeiro",
                                "sLast": "Último"
                            },
                            "oAria": {
                                "sSortAscending": ": Ordenar colunas de forma ascendente",
                                "sSortDescending": ": Ordenar colunas de forma descendente"
                            }
                        }
                    });
                }

                $(function () {
                    datatableInit();
                });

                //clique de escolha do select
                $("#ddlEstado").change(function () {

                    self.ShowLoad(true, "pFiltro");

                    var sigla = $("#ddlEstado").val();

                    var url = "../../DivisaoAdministrativa/GetMunicipioByUf?uf=" + sigla;

                    var ddlSource = "#ddlMunicipio";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Municipio</option>';
                                $("#ddlMunicipio").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlMunicipio").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Usuario',
                                    text: data,
                                    type: 'warning'
                                });
                            }
                        });

                    self.ShowLoad(false, "pFiltro");
                });

                //clique de escolha do select
                $("#ddlMunicipio").change(function () {

                    self.ShowLoad(true, "pFiltro");

                    var id = $("#ddlMunicipio").val();

                    var url = "../../Localidade/GetLocalidadeByMunicipio?id=" + id;

                    var ddlSource = "#ddlLocalidade";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Localidade</option>';
                                $("#ddlLocalidade").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlLocalidade").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Localidades',
                                    text: 'Localidades não encontradas.',
                                    type: 'warning'
                                });
                            }
                        });

                    self.ShowLoad(false, "pFiltro");
                });

                $("#ddlLocalidade").change(function () {
                    var id = $("#ddlLocalidade").val();

                    var url = "../../Aluno/GetAlunosByLocalidadeId?id=" + id;
                    $.getJSON(url,
                    { id: id },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Aluno</option>';
                                $("#ddlAluno").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlAluno").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Aluno',
                                    text: "Aluno não encontrado.",
                                    type: 'warning'
                                });
                            }
                        });
                });

                //clique de escolha do select
                $("#ddlProfissional").change(function () {
                    var profissionalId = $("#ddlProfissional").val();

                    var url = "../ControlePresenca/GetModalidadesByProfissionalId";

                    $.getJSON(url,
                        { id: profissionalId },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Atividade / Modalidade</option>';
                                $("#ddlModalidade").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlModalidade").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Profissional',
                                    text: "O Profissional selecionado não possui atividades / modalidades cadastradas.",
                                    type: 'warning'
                                });
                            }
                        });
                });

                //clique de escolha do select
                $("#ddlModalidade").change(function () {
                    var modalidadeId = $("#ddlModalidade").val();

                    var profissionalId = $("#ddlProfissional").val();

                    var url = "../Profissional/GetTurmasByModalidadeIdProfissionalId";

                    $.getJSON(url,
                        { modalidadeId: modalidadeId, profissionalId: profissionalId },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Turma</option>';
                                $("#ddlTurma").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlTurma").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Profissional',
                                    text: "O Profissional selecionado não possui turmas cadastradas.",
                                    type: 'warning'
                                });
                            }
                        });
                });

                //clique de escolha do select
                $("#ddlTurma").change(function () {

                    var id = $("#ddlTurma").val();

                    if (id === "") {
                        Site.Notification("Profissional", "Por favor selecione uma turma", "warning");
                    }

                    var url = "../Atividade/GetAtividadeById";

                    var urlDataTable = "../Atividade/GetAtividadeAlunosByAtividadeId";

                    axios.get(url, {
                        params: {
                            id: id
                        }
                    }).then(result => {
                        $("#divAlunos").show();
                        self.editDto.Categoria = result.data.nomeCategoria;
                        self.editDto.Estrutura = result.data.nomeEstrutura;
                        self.editDto.DiasSemana = result.data.diasSemana;
                        self.editDto.Horario = result.data.hrInicial + " - " + result.data.hrFinal;
                        $("#estrutura").val(result.data.nomeEstrutura);
                        $("#diaSemana").val(result.data.diasSemana);
                        $("#categoria").val(result.data.nomeCategoria);
                        $("#horario").val(result.data.hrInicial + " - " + result.data.hrFinal);


                        axios.get(urlDataTable, {
                            params: {
                                id: id
                            }
                        }).then(result => {
                            if (result.data.length > 0) {

                                self.editDto.Update = true;

                                $.each(result.data,
                                    function (i, item) {

                                        $('#alunoDataTable').DataTable().destroy();

                                        var table = $('#alunoDataTable').DataTable({
                                            columnDefs: [
                                                { "className": "text-center", "targets": "_all" }
                                            ],
                                            dom: '<"row"<"col-lg-6"l><"col-lg-6"f>><"table-responsive"t>p',
                                            "language": {
                                                "sEmptyTable": "Nenhum registro encontrado",
                                                "sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
                                                "sInfoEmpty": "Mostrando 0 até 0 de 0 registros",
                                                "sInfoFiltered": "(Filtrados de _MAX_ registros)",
                                                "sInfoPostFix": "",
                                                "sInfoThousands": ".",
                                                "sLengthMenu": "_MENU_ resultados por página",
                                                "sLoadingRecords": "Carregando...",
                                                "sProcessing": "Processando...",
                                                "sZeroRecords": "Nenhum registro encontrado",
                                                "sSearch": "Pesquisar: ",
                                                "oPaginate": {
                                                    "sNext": "Próximo →" +
                                                        "" +
                                                        "",
                                                    "sPrevious": "← Anterior",
                                                    "sFirst": "Primeiro",
                                                    "sLast": "Último"
                                                },
                                                "oAria": {
                                                    "sSortAscending": ": Ordenar colunas de forma ascendente",
                                                    "sSortDescending": ": Ordenar colunas de forma descendente"
                                                }
                                            }
                                        });

                                        table.row.add([
                                        "<div class='switch switch-sm switch-success'>" +
                                        "    <input type='checkbox' id='falta' name='falta-" + item.alunoId +"' data-plugin-ios-switch />" +
                                        "</div>",
                                        item.alunoId + " - " + item.nome,
                                        "<div class='input-group input-group-icon'>" +
                                        "    <textarea id='justificativa" + item.alunoId + "' name='justificativa-" + item.alunoId +"' rows='1' class='form-control form-control-lg'></textarea>" +
                                        "</div>"])  .draw();

                                        self.params.alunos.push(item.alunoId.toString());

                                        if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                                            $(function () {
                                                $('[data-plugin-ios-switch]').each(function () {
                                                    var $this = $(this);

                                                    $this.themePluginIOS7Switch();
                                                });
                                            });
                                        }
                                    });

                                $('input[name="arrAlunos"]').attr('value', self.params.alunos);
                            } else {

                                self.editDto.Update = false;
                            }
                        }).catch(error => {
                            Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                        });

                    }).catch(error => {
                        Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                    });
                });
            }
        }).apply(this, [jQuery]);
    },
    methods: {
        ShowLoad: function (flag, el) {
            var self = this;

            self.isLoading = flag;
            $("#" + el).loadingOverlay({
                "startShowing": flag
            });
            self.loading = flag;

            if (!flag) {
                self.isLoading = flag;
                $("#" + el).removeClass("loading-overlay-showing");
                self.loading = flag;
            } else {
                self.isLoading = flag;
                $("#" + el).addClass("loading-overlay-showing");
                self.loading = flag;
            }
        },
        DeleteControlePresenca: function (id) {
            var url = "ControlePresenca/Delete/" + id;
            $("#deleteControlePresencaHref").prop("href", url);
        },
        EditControlePresenca: function (id) {
            var self = this;

            axios.get("ControlePresenca/GetControlePresencaById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Controle = result.data.controle;
                self.editDto.Data = result.data.data;
                self.editDto.Justificativa = result.data.justificativa;
                self.editDto.NomeAluno = result.data.nomeAluno;
                self.editDto.MunicipioEstado = result.data.municipioEstado;
                self.editDto.NomeLocalidade = result.data.nomeLocalidade;
                self.editDto.AlunoId = result.data.alunoId;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
        handlePrintSubmit: function (itemId, mes) {
            if (itemId) {
                window.location.href = "/ControlePresenca/ImprimirFrequencia?id=" + itemId + "&mes=" + mes;
                return false; // Impede o submit do form
            } else {
                return true; // Permite o submit do form para impressão em lote
            }
        },
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteControlePresencaId"]').attr('value', id);
        $('#mdDeleteControlePresenca').modal('show');
        vm.DeleteControlePresenca(id)
    },
    EditModal: function (id) {
        $('input[name="editControlePresencaId"]').attr('value', id);
        $('#mdEditControlePresenca').modal('show');
        vm.EditControlePresenca(id)
    },
    ImprimirFrequencia: function (id) {
        $('#itemId').val(id);
        $('#mdMesImpressao').modal('show');
    },
};

function submitForm() {
    var itemId = $('#itemId').val();
    var mes = $('#ddlMes').val();

    if (!mes) {
        new PNotify({
            title: 'Atenção',
            text: 'Por favor, selecione um mês.',
            type: 'warning'
        });
        return false;
    }

    if (itemId) {
        // Impressão individual
        vm.handlePrintSubmit(mes);
    } else {
        // Impressão em lote
        $('#formImprimir').submit();
    }
}