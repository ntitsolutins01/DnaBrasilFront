var vm = new Vue({
    el: "#vLaudo",
    data: {
        loading: false,
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            // iosSwitcher
            (function ($) {

                'use strict';

                if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                    $(function () {
                        $('[data-plugin-ios-switch]').each(function () {
                            var $this = $(this);

                            $this.themePluginIOS7Switch();
                        });
                    });

                }

            }).apply(this, [jQuery]);

            var formid = $('form')[1].id;

            if (formid === "formPesquisarLaudo") {

                (function ($) {

                    'use strict';

                    var datatableInit = function () {

                        $('#datatable-default').dataTable({
                            order: [[0, 'desc']],
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

                $("#ddlLocalidade").change(function () {
                    var id = $("#ddlLocalidade").val();

                    var url = "../../Aluno/GetAlunosByLocalidade?id=" + id;

                    var ddlSource = "#ddlAluno";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
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
                                    title: 'Alunos',
                                    text: 'Alunos não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });
                });
            }

            if (formid === "formEditLaudo") {

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
                                    title: 'Fomento',
                                    text: 'Municípios não encontrados.',
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

                    var url = "../../Aluno/GetAlunosByLocalidade?id=" + id;

                    var ddlSource = "#ddlAluno";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
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
                                    title: 'Alunos',
                                    text: 'Alunos não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });

                    var urlProfissional = "../../Profissional/GetProfissionaisByLocalidade/?id=" + id;

                    var ddlSource = "#ddlProfissional";

                    $.getJSON(urlProfissional,
                        { id: $(ddlSource).val() },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Profissional</option>';
                                $("#ddlProfissional").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlProfissional").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Profissional',
                                    text: 'Profissional não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });
                });

                //mascara dos inputs 
                var $numeric = $(".numeric");
                $numeric.mask('00.00', { reverse: false });

                $("#formEditLaudo").validate({
                    rules: {
                        alturaSaude: {
                            required: true,
                            min: 1,
                            max: 300
                        },
                        massaCorporalSaude: {
                            required: true,
                            min: 1,
                            max: 200
                        },
                        envergaduraSaude: {
                            required: true,
                            min: 1,
                            max: 300
                        },
                        altura: {
                            required: true,
                            min: 1,
                            max: 300
                        },
                        massaCorporal: {
                            required: true,
                            min: 1,
                            max: 200
                        },
                        preensaoManual: {
                            required: true,
                            min: 1,
                            max: 150
                        },
                        flexibilidade: {
                            required: true,
                            min: 0,
                            max: 100
                        },
                        impulsaoHorizontal: {
                            required: true,
                            min: 0,
                            max: 500
                        },
                        testeVelocidade: {
                            required: true,
                            min: 0,
                            max: 60
                        },
                        aptidaoFisica: {
                            required: true,
                            min: 0,
                            max: 100
                        },
                        agilidade: {
                            required: true,
                            min: 0,
                            max: 60
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

            if (formid === "formLaudo") {


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
                                    title: 'Fomento',
                                    text: 'Municípios não encontrados.',
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

                    var url = "../../Aluno/GetAlunosByLocalidade?id=" + id;

                    var ddlSource = "#ddlAluno";

                    $.getJSON(url,
                        { id: $(ddlSource).val() },
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
                                    title: 'Alunos',
                                    text: 'Alunos não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });

                    var urlProfissional = "../../Profissional/GetProfissionaisByLocalidade/?id=" + id;

                    var ddlSource = "#ddlProfissional";

                    $.getJSON(urlProfissional,
                        { id: $(ddlSource).val() },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Profissional</option>';
                                $("#ddlProfissional").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlProfissional").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Profissional',
                                    text: 'Profissional não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });
                });

                //clique de escolha do select
                $("#ddlAluno").change(function () {
                    var id = $("#ddlAluno").val();

                    var url = "../../Aluno/GetAlunoIdadeById?id=" + id;

                    $.getJSON(url,
                        { id: id },
                        function (data) {
                            $("#divIdade").show();
                            $("#spanIdade").text(data + " anos");
                            if (data < 12) {
                                $("#liQualidade").hide();
                            }
                            if (data < 14) {
                                $("#liVocacional").hide();
                            }
                        });
                });

                //mascara dos inputs 
                var $numeric = $(".numeric");
                $numeric.mask('00.00', { reverse: false });

                $("#formLaudo").validate({
                    rules: {
                        // Saúde
                        alturaSaude: {
                            required: true,
                            min: 1,
                            max: 300
                        },
                        massaCorporalSaude: {
                            required: true,
                            min: 1,
                            max: 200
                        },
                        envergaduraSaude: {
                            required: true,
                            min: 1,
                            max: 300
                        },
                        // Talento Esportivo
                        altura: {
                            required: true,
                            min: 1,
                            max: 300
                        },
                        massaCorporal: {
                            required: true,
                            min: 1,
                            max: 200
                        },
                        preensaoManual: {
                            required: true,
                            min: 1,
                            max: 150
                        },
                        flexibilidade: {
                            required: true,
                            min: 0,
                            max: 100
                        },
                        impulsaoHorizontal: {
                            required: true,
                            min: 0,
                            max: 500
                        },
                        testeVelocidade: {
                            required: true,
                            min: 0,
                            max: 60
                        },
                        aptidaoFisica: {
                            required: true,
                            min: 0,
                            max: 100
                        },
                        agilidade: {
                            required: true,
                            min: 0,
                            max: 60
                        }
                    },
                    messages: {
                        // Saúde
                        alturaSaude: {
                            required: "Por favor, informe a altura",
                            min: "A altura deve ser maior que 1 cm",
                            max: "A altura deve ser menor que 300 cm"
                        },
                        massaCorporalSaude: {
                            required: "Por favor, informe o peso",
                            min: "O peso deve ser maior que 1 kg",
                            max: "O peso deve ser menor que 200 kg"
                        },
                        envergaduraSaude: {
                            required: "Por favor, informe a envergadura",
                            min: "A envergadura deve ser maior que 1 cm",
                            max: "A envergadura deve ser menor que 300 cm"
                        },
                        // Talento Esportivo
                        altura: {
                            required: "Por favor, informe a altura",
                            min: "A altura deve ser maior que 1 cm",
                            max: "A altura deve ser menor que 300 cm"
                        },
                        massaCorporal: {
                            required: "Por favor, informe o peso",
                            min: "O peso deve ser maior que 1 kg",
                            max: "O peso deve ser menor que 200 kg"
                        },
                        preensaoManual: {
                            required: "Por favor, informe a preensão manual",
                            min: "A preensão manual deve ser maior que 1 kg",
                            max: "A preensão manual deve ser menor que 150 kg"
                        },
                        flexibilidade: {
                            required: "Por favor, informe a flexibilidade",
                            min: "A flexibilidade deve ser maior ou igual a 0 cm",
                            max: "A flexibilidade deve ser menor que 100 cm"
                        },
                        impulsaoHorizontal: {
                            required: "Por favor, informe a impulsão horizontal",
                            min: "A impulsão horizontal deve ser maior ou igual a 0 cm",
                            max: "A impulsão horizontal deve ser menor que 500 cm"
                        },
                        testeVelocidade: {
                            required: "Por favor, informe o tempo do teste de velocidade",
                            min: "O tempo deve ser maior ou igual a 0 segundos",
                            max: "O tempo deve ser menor que 60 segundos"
                        },
                        aptidaoFisica: {
                            required: "Por favor, informe a aptidão física",
                            min: "A aptidão física deve ser maior ou igual a 0 mLO2/min",
                            max: "A aptidão física deve ser menor que 100 mLO2/min"
                        },
                        agilidade: {
                            required: "Por favor, informe o tempo de agilidade",
                            min: "O tempo deve ser maior ou igual a 0 segundos",
                            max: "O tempo deve ser menor que 60 segundos"
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
        Print: function () {
            var self = this;

            var filtros = {
                ddlFomento: $('#ddlFomento').val(),
                ddlEstado: $('#ddlEstado').val(),
                ddlMunicipio: $('#ddlMunicipio').val(),
                ddlLocalidade: $('#ddlLocalidade').val(),
                ddlAluno: $('#ddlAluno').val(),
                ddlTipoLaudo: $('#ddlTipoLaudo').val(),
                possuiFoto: $('#possuiFoto').val(),
                finalizado: $('#finalizado').val()
            };

            var queryString = Object.keys(filtros)
                .filter(key => filtros[key])
                .map(key => `${key}=${encodeURIComponent(filtros[key])}`)
                .join('&');

            var url = $(this).attr('href');
            if (queryString) {
                url += '?' + queryString;
            }

            window.open(url, '_blank');
        }
    }
});

var crud = {
    //DeleteModal: function (id) {
    //    $('input[name="LaudoId"]').attr('value', id);
    //    $('#mdDeleteLaudo').modal('show');
    //    vm.DeleteLaudo(id)
    //},
    //EditModal: function (id) {
    //    $('input[name="LaudoId"]').attr('value', id);
    //    $('#mdEditLaudo').modal('show');
    //    vm.EditLaudo(id)
    //}
};