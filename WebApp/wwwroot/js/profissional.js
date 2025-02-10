var vm = new Vue({
    el: "#vProfissional",
    data: {
        params: {
            cpf: "",
            ambientes: [],
            alunos: [],
            visible: false
        },
        loading: false,
        editDto: {
            Categoria: "", Estrutura: "", DiasSemana: "", Horario: "", Update: true
        }
    },
    mounted: function () {
        var self = this;
        (function ($) {

            'use strict';

            var formid = $('form')[1].id;

            //Inclusao
            if (formid === "formInclusaoProfissional") {
                //skin iosSwitcher
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

                    var url = "../DivisaoAdministrativa/GetMunicipioByUf?uf=" + sigla;

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
                                    text: data,
                                    type: 'warning'
                                });
                            }
                        });
                });

                //mascara dos inputs
                var $numCpf = $("#cpf");
                $numCpf.mask('000.000.000-00', { reverse: false });

                var $numCnpj = $("#cnpj");
                $numCnpj.mask('00.000.000/0000-00', { reverse: false });

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
                        cpf == "00000000000" ||
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

                $("#formInclusaoProfissional").validate({
                    rules: {
                        cpf: { cpf: true, required: true }
                    },
                    messages: {
                        cpf: { cpf: 'Formato de CPF inválido', required: "Por favor informe o número do CPF do profissional." }
                    },
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
            //Ediçao
            if (formid === "formEditProfissional") {

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

                //mascara dos inputs
                var $numCpf = $("#cpf");
                $numCpf.mask('000.000.000-00', { reverse: false });

                var $numCnpj = $("#cnpj");
                $numCnpj.mask('00.000.000/0000-00', { reverse: false });

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
                        cpf == "00000000000" ||
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

                $("#formEditProfissional").validate({
                    rules: {
                        cpf: { cpf: true, required: true }
                    },
                    messages: {
                        cpf: { cpf: 'Formato de CPF inválido', required: "Por favor informe o número do CPF do profissional." }
                    },
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
            //Profile
            if (formid === "formProfile") {

                //skin select
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

                $("#divAlunos").hide();

                //clique de escolha do select
                $("#ddlModalidade").change(function () {
                    var modalidadeId = $("#ddlModalidade").val();

                    var profissionalId = $("#profissionalIdMinhasTurmas").val();

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
                                    text: "O Profissional logado não possui atividades e turmas cadastradas.",
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
                                            ]
                                        });

                                        table.row.add([item.alunoId.toString(), item.alunoId + " - " + item.nome,
                                        "<a style='color:#F44336' href='javascript:(crud.DeleteAluno(\"" + item.alunoId + "\"))'><i class='fa fa-trash'></i></a>"])
                                            .draw();

                                        self.params.alunos.push(item.alunoId.toString());

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

                //mascara dos inputs
                var $numCpf = $("#cpf");
                $numCpf.mask('000.000.000-00', { reverse: false });

                var $numCnpj = $("#cnpj");
                $numCnpj.mask('00.000.000/0000-00', { reverse: false });

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
                        cpf == "00000000000" ||
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


                $("#formMinhasTurmas").validate({
                    rules: {
                        cpf: { cpf: true, required: true }
                    },
                    messages: {
                        cpf: { cpf: 'Formato de CPF inválido', required: "Por favor informe o número do CPF do profissional." }
                    },
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
            //Habilitar
            if (formid === "formHabilitarProfissional") {

                $("#formHabilitarProfissional").validate({
                    rules: {
                        "EndEmail": {
                            required: true,
                            email: true
                        }
                    },
                    messages: {
                        "EndEmail": {
                            required: "Por favor informe o endereço eletrônico válido do usuário.",
                            email: "Formato de e-mail inválido."
                        }
                    },
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
        DeleteProfissional: function (id) {
            var url = "Profissional/Delete/" + id;
            $("#deleteProfissionalHref").prop("href", url);
        },
        EditProfissional: function (id) {
            var self = this;

            axios.get("Profissional/GetProfissionalById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Nome = result.data.nome;
                self.editDto.Status = result.data.status;
                self.editDto.DtNascimento = result.data.dtnascimento;
                self.editDto.Email = result.data.email;
                self.editDto.AspNetUserId = result.data.aspnetuserid;
                self.editDto.Sexo = result.data.sexo;
                self.editDto.Cpf = result.data.cpf;
                self.editDto.Telefone = result.data.telefone;
                self.editDto.Celular = result.data.celular;
                self.editDto.Endereco = result.data.endereco;
                self.editDto.Numero = result.data.numero;
                self.editDto.Cep = result.data.cep;
                self.editDto.Bairro = result.data.bairro;
                self.editDto.Municipio = result.data.municipio;
                self.editDto.Ambientes = result.data.ambientes;
                self.editDto.Contratos = result.data.contratos;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
        ExisteCpf: function () {
            var self = this;
            self.ShowLoad(true, "vUsuario");

            axios.get("GetUsuarioByCpf/?cpf=" + self.params.cpf).then(result => {

                if (result.data === false) {
                    new PNotify({
                        title: 'Usuario',
                        text: "Já existe um usuário cadastrado com esse cpf.",
                        type: 'warning'
                    });
                }

                self.ShowLoad(false, "vUsuario");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.response.data, "error", 1);
                self.ShowLoad(false, "vUsuario");
            });
        },
        ExisteEmail: function () {
            var self = this;
            self.ShowLoad(true, "vUsuario");

            axios.get("GetUsuarioByEmail/?email=" + self.params.email).then(result => {

                if (result.data === false) {
                    new PNotify({
                        title: 'Usuario',
                        text: "Já existe um usuário cadastrado com esse email.",
                        type: 'warning'
                    });
                }

                self.ShowLoad(false, "vUsuario");

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.response.data, "error", 1);
                self.ShowLoad(false, "vUsuario");
            });
        },
        DesvincularAlunos: function () {
            axios.get("Curso/GetProfissionaisById/?id=" + id).then(result => {

                if (result.data.listProfissionais.length > 0) {
                    var items = '<option value="">Selecionar o Profissional</option>';
                    $("#ddlProfissional").empty;
                    $.each(result.data.listProfissionais,
                        function (i, row) {
                            if (row.selected) {
                                items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                            } else {
                                items += "<option value='" + row.value + "'>" + row.text + "</option>";
                            }
                        });
                    $("#ddlProfissional").html(items);
                }
                else {
                    new PNotify({
                        title: 'Profissional',
                        text: 'Profissionais não encontrados.',
                        type: 'warning'
                    });
                }

            }).catch(error => {
                console.error('Erro ao carregar dados:', error);
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
        AddAluno: function () {
            var self = this;
            self.ShowLoad(true, "divAlunos");

            var mapped = $("#ddlAluno").select2('data');

            if (self.params.alunos.indexOf(mapped[0].id) !== -1) {
                self.ShowLoad(false, "divAlunos");
                new PNotify({
                    title: 'Aluno',
                    text: 'Aluno já foi adicionado anteriormente.',
                    type: 'warning'
                });
                return;
            }

            $('#alunoDataTable').DataTable().destroy();

            var table = $('#alunoDataTable').DataTable({
                columnDefs: [
                    { "className": "text-center", "targets": "_all" }
                ]
            });

            table.row.add([mapped[0].id, mapped[0].text,
            "<a style='color:#F44336' href='javascript:(crud.DeleteAluno(\"" + mapped[0].id + "\"))'><i class='fa fa-trash'></i></a>"])
                .draw();

            self.params.alunos.push(mapped[0].id);

            $('input[name="arrAlunos"]').attr('value', self.params.alunos);

            $("#ddlAluno").select2("val", "0");

            self.ShowLoad(false, "divAlunos");
        },
        DeleteAluno: function (id) {
            var self = this;
            self.ShowLoad(true, "divAlunos");

            var table = $('#alunoDataTable').DataTable();
            table.rows(function (idx, data, node) {
                return data[0] === id;
            })
                .remove()
                .draw();

            const alunos = self.params.alunos;

            const index = alunos.indexOf(id);

            if (index !== -1) {
                alunos.splice(index, 1);
            }

            self.params.alunos = alunos;

            $('input[name="arrAlunos"]').attr('value', self.params.alunos);

            $("#ddlAluno").select2("val", "0");
            self.ShowLoad(false, "divAlunos");
        }
    }
});
var crud = {
    AddAluno: function () {
        vm.AddAluno()
    },
    DeleteAluno: function (index) {
        vm.DeleteAluno(index.toString())
    },
    DeleteModal: function (id) {
        $('input[name="deleteProfissionalId"]').attr('value', id);
        $('#mdDeleteProfissional').modal('show');
        vm.DeleteProfissional(id)
    },
    DesvincularAlunoModal: function (id) {
        $('input[name="profissionalId"]').attr('value', id);
        $('#mdDesvincularAlunos').modal('show');
        vm.DesvincularAlunos(id)
    }
};
