var vm = new Vue({
    el: "#formDadosAluno",
    data: {
        comboItems: [],
        selectedItem: "",
        params: {
            cpf: "",
            email: "",
            deficiencias: [],
            modalidadeAlunos: [],
            possuiDeficiencia: false
        },
        loading: false,
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            this.selectedItem = "Selecionar Etapa de Ensino";

            var formid = $('form')[1].id;

            //inclusão
            if (formid === "formDadosAluno") {


                if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                    $(function () {
                        $('[data-plugin-ios-switch]').each(function () {
                            var $this = $(this);

                            $this.themePluginIOS7Switch();
                        });
                    });
                }

                //skin checkbox
                if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                    $(function () {
                        $('[data-plugin-ios-switch]').each(function () {
                            var $this = $(this);

                            $this.themePluginIOS7Switch();
                        });
                    });
                }
                //skin select
                // MultiSelect
                (function (theme, $) {

                    theme = theme || {};

                    var instanceName = '__multiselect';

                    var PluginMultiSelect = function ($el, opts) {
                        return this.initialize($el, opts);
                    };

                    PluginMultiSelect.defaults = {
                        templates: {
                            filter: '<div class="input-group"><span class="input-group-addon"><i class="fa fa-search"></i></span><input class="form-control multiselect-search" type="text"></div>'
                        }
                    };

                    PluginMultiSelect.prototype = {
                        initialize: function ($el, opts) {
                            if ($el.data(instanceName)) {
                                return this;
                            }

                            this.$el = $el;

                            this
                                .setData()
                                .setOptions(opts)
                                .build();

                            return this;
                        },

                        setData: function () {
                            this.$el.data(instanceName, this);

                            return this;
                        },

                        setOptions: function (opts) {
                            this.options = $.extend(true, {}, PluginMultiSelect.defaults, opts);

                            return this;
                        },

                        build: function () {
                            this.$el.multiselect(this.options);

                            return this;
                        }
                    };

                    // expose to scope
                    $.extend(theme, {
                        PluginMultiSelect: PluginMultiSelect
                    });

                    // jquery plugin
                    $.fn.themePluginMultiSelect = function (opts) {
                        return this.each(function () {
                            var $this = $(this);

                            if ($this.data(instanceName)) {
                                return $this.data(instanceName);
                            } else {
                                return new PluginMultiSelect($this, opts);
                            }

                        });
                    }

                }).apply(this, [window.theme, jQuery]);

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

                //clique de escolha do select
                $("#ddlEstado").change(function () {
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
                });
                //clique de escolha do select
                $("#ddlMunicipio").change(function () {
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
                });

                //clique de escolha do select
                $("#ddlLocalidade").change(function () {
                    var id = $("#ddlLocalidade").val();

                    var url = "../../Profissional/GetProfissionaisByLocalidade/?id=" + id;

                    var ddlSource = "#ddlProfissionalAluno";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Profissional</option>';
                                $("#ddlProfissionalAluno").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlProfissionalAluno").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Profissional',
                                    text: 'Profissional não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });

                    var urlLocalidadeFomento = "../../Aluno/GetFomentoByLocalidadeId/";

                    axios.get(urlLocalidadeFomento, {
                        params: {
                            id: id
                        }
                    }).then(result => {
                        console.log('Dados retornados:', result.data);

                        if (result.data && result.data.length > 0) {
                            var items = '<option value="">Selecionar o Fomento</option>';
                            $("#ddlFomento").empty();
                            $.each(result.data,
                                function (i, row) {
                                    if (row.selected) {
                                        items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                                    } else {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    }
                                });
                            $("#ddlFomento").html(items);
                        } else {
                            new PNotify({
                                title: 'Fomento/Contrato',
                                text: 'Fomento/Contrato não encontrado.',
                                type: 'warning'
                            });
                        }
                    }).catch(error => {
                        console.error('Erro ao carregar dados:', error);
                        Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                    }).finally(function () {
                        // sempre será executado
                    });
                });

                //clique de escolha do select
                $("#ddlEtapa").change(function () {

                    var etapaId = $("#ddlEtapa").val();
                    var localidadeId = $("#ddlLocalidade").val();

                    if (localidadeId === "") {
                        new PNotify({
                            title: 'Aluno',
                            text: 'Por favor selecione a localidade.',
                            type: 'warning'
                        });
                        return;
                    }

                    if (etapaId === "") {
                        new PNotify({
                            title: 'Aluno',
                            text: 'Por favor selecione a etapa de ensino.',
                            type: 'warning'
                        });
                        return;
                    }

                    var url = "../../Serie/GetSeriesByLocalidadeIdEtapaId/";

                    axios.get(url, {
                        params: {
                            localidadeId: localidadeId,
                            etapaId: etapaId
                        }
                    }).then(result => {
                        console.log('Dados retornados:', result.data);

                        if (result.data && result.data.length > 0) {
                            var items = '<option value="">Selecionar a série</option>';
                            $("#ddlSerie").empty();
                            $.each(result.data,
                                function (i, row) {
                                    if (row.selected) {
                                        items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                                    } else {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    }
                                });
                            $("#ddlSerie").html(items);
                        } else {
                            new PNotify({
                                title: 'Aluno',
                                text: 'Séries não encontradas.',
                                type: 'warning'
                            });
                        }
                    }).catch(error => {
                        console.error('Erro ao carregar dados:', error);
                        Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                    }).finally(function () {
                        // sempre será executado
                    });;
                });

                //clique de escolha do select
                $("#ddlSerie").change(function () {

                    var serie = $("#ddlSerie").val();
                    var etapaId = $("#ddlEtapa").val();
                    var localidadeId = $("#ddlLocalidade").val();

                    if (localidadeId === "") {
                        new PNotify({
                            title: 'Aluno',
                            text: 'Por favor selecione a localidade.',
                            type: 'warning'
                        });
                        return;
                    }

                    if (etapaId === "") {
                        new PNotify({
                            title: 'Aluno',
                            text: 'Por favor selecione a etapa de ensino.',
                            type: 'warning'
                        });
                        return;
                    }

                    if (serie === "") {
                        new PNotify({
                            title: 'Aluno',
                            text: 'Por favor selecione a série',
                            type: 'warning'
                        });
                        return;
                    }

                    var url = "../../Serie/GetTurmasByLocalidadeIdEtapaIdSerie/";

                    axios.get(url, {
                        params: {
                            localidadeId: localidadeId,
                            etapaId: etapaId,
                            serie: serie
                        }
                    }).then(result => {
                        console.log('Dados retornados:', result.data);

                        if (result.data && result.data.length > 0) {
                            var items = '<option value="">Selecionar a turma</option>';
                            $("#ddlTurma").empty();
                            $.each(result.data,
                                function (i, row) {
                                    if (row.selected) {
                                        items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                                    } else {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    }
                                });
                            $("#ddlTurma").html(items);
                        } else {
                            new PNotify({
                                title: 'Aluno',
                                text: 'Turmas não encontradas.',
                                type: 'warning'
                            });
                        }
                    }).catch(error => {
                        console.error('Erro ao carregar dados:', error);
                        Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                    }).finally(function () {
                        // sempre será executado
                    });
                });

                //mascara dos inputs
                var $numCpf = $("#cpf");
                $numCpf.mask('000.000.000-00', { reverse: false });

                var $numTel = $("#numTelefone");
                $numTel.mask('(00) 0000-0000');

                var $numTel = $("#numCelular");
                $numTel.mask('(00) 00000-0000');

                var $numCep = $("#cep");
                $numCep.mask('00000-000');

                var $numDtNasc = $("#DtNascimento");
                $numDtNasc.mask('00/00/0000', { reverse: false });

                jQuery.validator.addMethod("cpf", function (cpf, element) {
                    var regex = /^\d{3}\.\d{3}\.\d{3}\-\d{2}$/;
                    var add, rev, i;
                    if (!regex.test(cpf))
                        return false;

                    cpf = cpf.replace(/[^\d]+/g, '');
                    if (cpf == '') return false;
                    // Elimina CPFs invalidos conhecidos	
                    if (cpf.length != 11 ||
                        cpf == "11111111111" ||
                        cpf == "22222222222" ||
                        cpf == "33333333333" ||
                        cpf == "44444444444" ||
                        cpf == "55555555555" ||
                        cpf == "66666666666" ||
                        cpf == "77777777777" ||
                        cpf == "88888888888" ||
                        cpf == "99999999999")
                        return false;
                    // Valida 1o digito	
                    add = 0;
                    for (i = 0; i < 9; i++)
                        add += parseInt(cpf.charAt(i)) * (10 - i);
                    rev = 11 - (add % 11);
                    if (rev == 10 || rev == 11)
                        rev = 0;
                    if (rev != parseInt(cpf.charAt(9)))
                        return false;
                    // Valida 2o digito	
                    add = 0;
                    for (i = 0; i < 10; i++)
                        add += parseInt(cpf.charAt(i)) * (11 - i);
                    rev = 11 - (add % 11);
                    if (rev == 10 || rev == 11)
                        rev = 0;
                    if (rev != parseInt(cpf.charAt(10)))
                        return false;
                    return true;


                }, "Informe um CPF válido");

                $("#formDadosAluno").validate({
                    highlight: function (label) {
                        $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
                    },
                    success: function (label) {
                        $(label).closest('.form-group').removeClass('has-error');
                        label.remove();
                    },
                    errorPlacement: function (error, element) {
                        var placement = element.closest('.input-group');
                        if (!placement.get(0)) {
                            placement = element;
                        }
                        if (error.text() !== '') {
                            placement.after(error);
                        }
                    }
                });

                $("#formCreateGrauParentesco").validate({
                    highlight: function (label) {
                        $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
                    },
                    success: function (label) {
                        $(label).closest('.form-group').removeClass('has-error');
                        label.remove();
                    },
                    errorPlacement: function (error, element) {
                        var placement = element.closest('.input-group');
                        if (!placement.get(0)) {
                            placement = element;
                        }
                        if (error.text() !== '') {
                            placement.after(error);
                        }
                    }
                });
            }

            var datatableInit = function () {

                $('.adicionados').dataTable({
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

            };

            $(function () {
                datatableInit();
            });

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
        DeleteDadosAluno: function (id) {
            var url = "DadosAluno/Delete/" + id;
            $("#deleteDadosAlunoHref").prop("href", url);
        },
        ExisteCpf: function () {
            var self = this;
            self.ShowLoad(true, "pAluno");

            axios.get("GetAlunoByCpf",
                {
                    params: {
                        cpf: self.params.cpf
                    }
                })
                .then(result => {

                    if (result.data === false) {
                        Site.Notification("Aluno", "Já existe um aluno cadastrado com esse cpf.", "warning", 2);
                    } else if (result.data !== true) {
                        Site.Notification("Aluno", result.data, "warning", 2);
                        self.ShowLoad(false, "pAluno");
                    }

                    self.ShowLoad(false, "pAluno");

                }).catch(error => {
                    Site.Notification("Erro ao buscar e analisar dados", error.response.data, "error", 2);
                    self.ShowLoad(false, "pAluno");
                });
        },
        ExisteEmail: function () {
            var self = this;
            self.ShowLoad(true, "pAluno");

            axios.get("GetAlunoByEmail",
                {
                    params: {
                        email: self.params.email
                    }
                })
                .then(result => {

                    if (result.data === false) {
                        Site.Notification("Aluno", "Já existe um aluno cadastrado com esse email.", "warning", 2);
                    } else if (result.data !== true) {
                        Site.Notification("Aluno", result.data, "warning", 2);
                        self.ShowLoad(false, "pAluno");
                    }

                    self.ShowLoad(false, "pAluno");

                }).catch(error => {
                    Site.Notification("Erro ao buscar e analisar dados", error.response.data, "error", 2);
                    self.ShowLoad(false, "pAluno");
                });
        },
        AddDeficiencia: function () {
            var self = this;

            var mapped = $("#ddlDeficiencia").select2('data');

            if (self.params.deficiencias.indexOf(mapped[0].id) !== -1) {

                new PNotify({
                    title: 'Deficiência',
                    text: 'Deficiência já foi adicionada anteriormente.',
                    type: 'warning'
                });
                return;
            }

            $('#deficienciaDataTable').DataTable().destroy();

            var table = $('#deficienciaDataTable').DataTable({
                columnDefs: [
                    { "className": "text-center", "targets": "_all" }
                ]
            });

            table.row.add([mapped[0].id, mapped[0].text,
            "<a style='color:#F44336' href='javascript:(crud.DeleteDeficiencia(\"" + mapped[0].id + "\"))'><i class='fa fa-trash'></i></a>"])
                .draw();

            self.params.deficiencias.push(mapped[0].id);

            $('input[name="arrDeficiencias"]').attr('value', self.params.deficiencias);

            $("#ddlDeficiencia").select2("val", "0");

        },
        AddModalidadeAluno: function () {
            var self = this;

            var mapped = $("#ddlModalidadeAluno").select2('data');

            if (self.params.modalidadeAlunos.indexOf(mapped[0].id) !== -1) {

                new PNotify({
                    title: 'Modalidade',
                    text: 'Modalidade já foi adicionado anteriormente.',
                    type: 'warning'
                });
                return;
            }

            $('#modalidadeAlunoDataTable').DataTable().destroy();

            var table = $('#modalidadeAlunoDataTable').DataTable({
                columnDefs: [
                    { "className": "text-center", "targets": "_all" }
                ]
            });

            table.row.add([mapped[0].id, mapped[0].text,
            "<a style='color:#F44336' href='javascript:(crud.DeleteModalidadeAluno(\"" + mapped[0].id + "\"))'><i class='fa fa-trash'></i></a>"])
                .draw();

            self.params.modalidadeAlunos.push(mapped[0].id);

            $('input[name="arrModalidadeAlunos"]').attr('value', self.params.modalidadeAlunos);

            $("#ddlModalidadeAluno").select2("val", "0");

        },
        DeleteDeficiencia: function (id) {
            var self = this;

            var table = $('#deficienciaDataTable').DataTable();
            table.rows(function (idx, data, node) {
                return data[0] === id;
            })
                .remove()
                .draw();

            $("#ddlDeficiencia").select2("val", "0");
        },
        DeleteModalidadeAluno: function (id) {
            var self = this;

            var table = $('#modalidadeAlunoDataTable').DataTable();
            table.rows(function (idx, data, node) {
                return data[0] === id;
            })
                .remove()
                .draw();

            $("#ddlModalidadeAluno").select2("val", "0");
        },
        CreateEtapaEnsino: function (etapa) {
            var nome = $("#nomeEtapaEnsino").val();
            var url = "CreateEtapaEnsino";
            $.getJSON(url,
                { nome: nome },
                function (data) {
                    if (data.length > 0) {
                        var items = '<option value="">Selecionar Etapa Ensino</option>';
                        $("#ddlEtapa").empty;
                        $.each(data,
                            function (i, row) {
                                items += "<option value='" + row.value + "'>" + row.text + "</option>";
                            });
                        $("#ddlEtapa").html(items);
                    }
                    else {
                        new PNotify({
                            title: 'Curso',
                            text: 'Cursos não encontrados.',
                            type: 'warning'
                        });
                    }
                });
        },
        CreateGrauParentesco: function(grau) {

            var url = "../../Aluno/CreateGrauParentesco/";

            $.getJSON(url,
                { nome: grau },
                function (data) {
                    if (data.length > 0) {
                        var items = '<option value="">Selecionar Grau de Parentêsco</option>';
                        $("#ddlGrauParentesco").empty;
                        $.each(data,
                            function (i, row) {
                                items += "<option value='" + row.value + "'>" + row.text + "</option>";
                            });
                        $("#ddlGrauParentesco").html(items);
                        $('#mdGrauParentesco').modal('hide');
                        new PNotify({
                            title: 'Aluno',
                            text: 'Grau de Parentêsco cadastrado com sucesso.',
                            type: 'success'
                        });
                    }
                    else {
                        new PNotify({
                            title: 'Aluno',
                            text: 'Grau de Parentêsco não encontrados.',
                            type: 'warning'
                        });
                        $('#mdGrauParentesco').modal('hide');
                    }
                });
        }
    }
});

var crud = {
    AddEtapaEnsino: function () {
        $('#mdEtapaEnsino').modal('show');
    },
    CreateEtapaEnsino: function () {
        
        vm.CreateEtapaEnsino();
    },
    CreateGrauParentesco: function (grau) {
        //console.log(grau);
        vm.CreateGrauParentesco(grau);
    }
};