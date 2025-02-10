var vm = new Vue({
    el: "#vModalidade",
    data: {
        loading: false,
        editDto: { Id: "", LinhaAcao: "", Nome: "", Vo2MaxIni: "", Vo2MaxFim: "", VinteMetrosIni: "", VinteMetrosFim: "", ShutlleRunIni: "", ShutlleRunFim: "", FlexibilidadeIni: "", FlexibilidadeFim: "", PreensaoManualIni: "", PreensaoManualFim: "", AbdominalPranchaIni: "", AbdominalPranchaFim: "", ImpulsaoIni: "", ImpulsaoFim: "", EnvergaduraIni: "", EnvergaduraFim: "", PesoIni: "", PesoFim: "", AlturaIni: "", AlturaFim: "", Status: true, ByteImage: null }
    },
    mounted: function () {
        var self = this;
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

            var formid = $('form')[1].id;

            if (formid === "formEditModalidade") {

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

                if ($.isFunction($.fn['tooltip'])) {
                    $('[data-toggle=tooltip],[rel=tooltip]').tooltip({ container: 'body' });
                }

                $("#formEditModalidade").validate({
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

            if (formid === "formModalidade") {

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

                if ($.isFunction($.fn['tooltip'])) {
                    $('[data-toggle=tooltip],[rel=tooltip]').tooltip({ container: 'body' });
                }

                $("#formModalidade").validate({
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
        DeleteModalidade: function (id) {
            var url = "Modalidade/Delete/" + id;
            $("#deleteModalidadeHref").prop("href", url);
        },
        EditModalidade: function (id) {
            var self = this;

            axios.get("Modalidade/GetModalidadeById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Nome = result.data.nome;
                self.editDto.Status = result.data.status;
                self.editDto.Vo2MaxIni = result.data.vo2MaxIni;
                self.editDto.Vo2MaxFim = result.data.vo2MaxFim;
                self.editDto.VinteMetrosIni = result.data.vinteMetrosIni;
                self.editDto.VinteMetrosFim = result.data.vinteMetrosFim;
                self.editDto.ShutlleRunIni = result.data.shutlleRunIni;
                self.editDto.ShutlleRunFim = result.data.shutlleRunFim;
                self.editDto.FlexibilidadeIni = result.data.flexibilidadeIni;
                self.editDto.FlexibilidadeFim = result.data.flexibilidadeFim;
                self.editDto.PreensaoManualIni = result.data.preensaoManualIni;
                self.editDto.PreensaoManualFim = result.data.preensaoManualFim;
                self.editDto.AbdominalPranchaIni = result.data.abdominalPranchaIni;
                self.editDto.AbdominalPranchaFim = result.data.abdominalPranchaFim;
                self.editDto.ImpulsaoIni = result.data.impulsaoIni;
                self.editDto.ImpulsaoFim = result.data.impulsaoFim;
                self.editDto.EnvergaduraIni = result.data.envergaduraIni;
                self.editDto.EnvergaduraFim = result.data.envergaduraFim;
                self.editDto.PesoIni = result.data.pesoIni;
                self.editDto.PesoFim = result.data.pesoFim;
                self.editDto.AlturaIni = result.data.alturaIni;
                self.editDto.AlturaFim = result.data.alturaFim;
                self.editDto.ByteImage = result.data.byteImage;

                if (result.data.listLinhasAcoes && result.data.listLinhasAcoes.length > 0) {
                    var items = '<option value="">Selecionar a Linha Ação</option>';
                    $("#ddlLinhaAcao").empty();
                    $.each(result.data.listLinhasAcoes,
                        function (i, row) {
                            if (row.selected) {
                                items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                            } else {
                                items += "<option value='" + row.value + "'>" + row.text + "</option>";
                            }
                        });
                    $("#ddlLinhaAcao").html(items);
                } else {
                    new PNotify({
                        title: 'Linha Ação',
                        text: 'Linhas Ações não encontradas.',
                        type: 'warning'
                    });
                }

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        }
    }
});

var crud = {
    DeleteModal: function (id) {
        $('input[name="ModalidadeId"]').attr('value', id);
        $('#mdDeleteModalidade').modal('show');
        vm.DeleteModalidade(id)
    },
    EditModal: function (id) {
        $('input[name="ModalidadeId"]').attr('value', id);
        $('#mdEditModalidade').modal('show');
        vm.EditModalidade(id)
    }
};