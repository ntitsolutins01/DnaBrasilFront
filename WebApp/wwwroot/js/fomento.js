var vm = new Vue({
    el: "#vFomento",
    data: {
        loading: false,
        editDto: { Nome: "", Codigo: "", Status: "", DtIni: "", DtFim: "" }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            var formid = $('form')[1].id;

            if (formid === "formFomento") {

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


                $("#formFomento").validate({
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

                    var url = "../../Localidade/GetLocalidadeByMunicipio";

                    $.getJSON(url,
                        { id: id },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Localidade</option>';
                                
                                for (var i = 0; i != data.length; i++) {
                                    $('select#ddlLocalidade').append('<option value="' + data[i].value + '">' + data[i].text + '</option>');
                                }

                                $('select#ddlLocalidade').multiselect('rebuild');
                            }
                            else {
                                new PNotify({
                                    title: 'Localidades',
                                    text: 'Localidades não encontradas.',
                                    type: 'warning'
                                });
                            }
                        }
                    );
                });



                //clique de escolha do select
                //$("#ddlLocalidade").change(function () {
                //    var id = $("#ddlLocalidade").val();

                //    axios.get("../Profissional/GetProfissionaisByLocalidade/?id=" + id).then(result => {

                //        var profissionais = result.data;

                //        $('#profissionalDataTable').DataTable().destroy();

                //        $('#profissionalDataTable').DataTable({
                //            data: profissionais,
                //            "columns": [
                //                { "data": "id" },
                //                { "data": "nome" }
                //            ],
                //            columnDefs: [
                //                { "className": "text-center", "targets": "_all" }
                //            ],
                //            order: [[1, 'asc']],
                //            "paging": true,
                //            "searching": true,
                //            "language": {
                //                "sEmptyTable": "Nenhum registro encontrado",
                //                "sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
                //                "sInfoEmpty": "Mostrando 0 até 0 de 0 registros",
                //                "sInfoFiltered": "(Filtrados de _MAX_ registros)",
                //                "sInfoPostFix": "",
                //                "sInfoThousands": ".",
                //                "sLengthMenu": "_MENU_ resultados por página",
                //                "sLoadingRecords": "Carregando...",
                //                "sProcessing": "Processando...",
                //                "sZeroRecords": "Nenhum registro encontrado",
                //                "sSearch": "Pesquisar: ",
                //                "select": {
                //                    "rows": {
                //                        "_": "Você selecionou %d linhas",
                //                        "0": "Clique em uma linha para selecioná-la",
                //                        "1": "Apenas 1 linha selecionada"
                //                    }
                //                },
                //                "oPaginate": {
                //                    "sNext": "Próximo →" +
                //                        "" +
                //                        "",
                //                    "sPrevious": "← Anterior",
                //                    "sFirst": "Primeiro",
                //                    "sLast": "Último"
                //                },
                //                "oAria": {
                //                    "sSortAscending": ": Ordenar colunas de forma ascendente",
                //                    "sSortDescending": ": Ordenar colunas de forma descendente"
                //                }
                //            }
                //        });

                //        $('#profissionalDataTable').DataTable().draw();

                //    }).catch(error => {
                //        Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                //    });
                //});


            }
            if (formid === "formEditFomento") {

                //Switch select Status
                if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                    $(function () {
                        $('[data-plugin-ios-switch]').each(function () {
                            var $this = $(this);

                            $this.themePluginIOS7Switch();
                        });
                    });
                }

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


                $("#formEditFomento").validate({
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
                //$("#ddlLocalidade").change(function () {
                //    var id = $("#ddlLocalidade").val();

                //    //axios.get("../Profissional/GetProfissionaisByLocalidade/?id=" + id).then(result => {

                //    //    var profissionais = result.data;

                //    //    $('#profissionalDataTable').DataTable().destroy();

                //    //    $('#profissionalDataTable').DataTable({
                //    //        data: profissionais,
                //    //        "columns": [
                //    //            { "data": "id" },
                //    //            { "data": "nome" }
                //    //        ],
                //    //        columnDefs: [
                //    //            { "className": "text-center", "targets": "_all" }
                //    //        ],
                //    //        order: [[1, 'asc']],
                //    //        "paging": true,
                //    //        "searching": true,
                //    //        "language": {
                //    //            "sEmptyTable": "Nenhum registro encontrado",
                //    //            "sInfo": "Mostrando de _START_ até _END_ de _TOTAL_ registros",
                //    //            "sInfoEmpty": "Mostrando 0 até 0 de 0 registros",
                //    //            "sInfoFiltered": "(Filtrados de _MAX_ registros)",
                //    //            "sInfoPostFix": "",
                //    //            "sInfoThousands": ".",
                //    //            "sLengthMenu": "_MENU_ resultados por página",
                //    //            "sLoadingRecords": "Carregando...",
                //    //            "sProcessing": "Processando...",
                //    //            "sZeroRecords": "Nenhum registro encontrado",
                //    //            "sSearch": "Pesquisar: ",
                //    //            "select": {
                //    //                "rows": {
                //    //                    "_": "Você selecionou %d linhas",
                //    //                    "0": "Clique em uma linha para selecioná-la",
                //    //                    "1": "Apenas 1 linha selecionada"
                //    //                }
                //    //            },
                //    //            "oPaginate": {
                //    //                "sNext": "Próximo →" +
                //    //                    "" +
                //    //                    "",
                //    //                "sPrevious": "← Anterior",
                //    //                "sFirst": "Primeiro",
                //    //                "sLast": "Último"
                //    //            },
                //    //            "oAria": {
                //    //                "sSortAscending": ": Ordenar colunas de forma ascendente",
                //    //                "sSortDescending": ": Ordenar colunas de forma descendente"
                //    //            }
                //    //        }
                //    //    });

                //    //    $('#profissionalDataTable').DataTable().draw();

                //    //}).catch(error => {
                //    //    Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                //    //});
                //});
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
        DeleteFomento: function (id) {
            var url = "Fomento/Delete/" + id;
            $("#deleteFomentoHref").prop("href", url);
        },
        EditFomento: function (id) {
            var self = this;

            axios.get("Fomento/GetFomentoById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Codigo = result.data.codigo;
                self.editDto.MunicipioEstado = result.data.municipioEstado;
                self.editDto.Localidade = result.data.localidade;
                self.editDto.Nome = result.data.nome;
                self.editDto.DtIni = result.data.dtIni;
                self.editDto.DtFim = result.data.dtFim;
                self.editDto.Status = result.data.status;
            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="FomentoId"]').attr('value', id);
        $('#mdDeleteFomento').modal('show');
        vm.DeleteFomento(id)
    },
    EditModal: function (id) {
        $('input[name="FomentoId"]').attr('value', id);
        $('#mdEditFomento').modal('show');
        vm.EditFomento(id)
    }
};