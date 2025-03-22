var vm = new Vue({
    el: "#vInventario",
    data: {
        loading: false,
        editDto: {
            Id: "",
            GrupoMaterialId: "",
            TipoMaterialId: "",
            MaterialId: "",
            LocalidadeId: "",
            Quantidade: ""
        },
        arquivosInventarios: []
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            // Skin checkbox initialization
            if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {
                $(function () {
                    $('[data-plugin-ios-switch]').each(function () {
                        var $this = $(this);
                        $this.themePluginIOS7Switch();
                    });
                });
            }

            var formid = $('form')[1].id;

            if (formid === "formEditInventario") {
                $("#formEditInventario").validate({
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

            if (formid === "formInventario" || formid === "formPesquisarInventario") {
                // Initialize select2 elements
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
                $select.on('change', function () {
                    $(this).trigger('blur');
                });

                $("#ddlGrupoMaterial").change(function () {
                    self.ShowLoad(true, "pFiltro");
                    var url = "../../TipoMaterial/GetTiposMateriaisByGrupoMaterialId";
                    var ddlSource = "#ddlGrupoMaterial";
                    $.getJSON(url, { id: $(ddlSource).val() }, function (data) {
                        if (data.length > 0) {
                            var items = '<option value="">Selecionar Tipo de Material</option>';
                            $("#ddlTipoMaterial").empty;
                            $.each(data, function (i, row) {
                                items += "<option value='" + row.value + "'>" + row.text + "</option>";
                            });
                            $("#ddlTipoMaterial").html(items);
                        }
                        else {
                            new PNotify({
                                title: 'Tipo de Material',
                                text: data,
                                type: 'warning'
                            });
                        }
                    });
                    self.ShowLoad(false, "pFiltro");
                });

                $("#ddlTipoMaterial").change(function () {
                    self.ShowLoad(true, "pFiltro");
                    var url = "../../Material/GetMateriaisByTipoMaterialId";
                    var ddlSource = "#ddlTipoMaterial";
                    $.getJSON(url, { id: $(ddlSource).val() }, function (data) {
                        if (data.length > 0) {
                            var items = '<option value="">Selecionar Material</option>';
                            $("#ddlMaterial").empty;
                            $.each(data, function (i, row) {
                                items += "<option value='" + row.value + "'>" + row.text + "</option>";
                            });
                            $("#ddlMaterial").html(items);
                        }
                        else {
                            new PNotify({
                                title: 'Material',
                                text: data,
                                type: 'warning'
                            });
                        }
                    });
                    self.ShowLoad(false, "pFiltro");
                });

                $("#formInventario").validate({
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
            this.loading = flag;
            $("#" + el).loadingOverlay({ "startShowing": flag });
            if (!flag) {
                $("#" + el).removeClass("loading-overlay-showing");
            } else {
                $("#" + el).addClass("loading-overlay-showing");
            }
        },
        loadArquivosInventario: function (id) {
            var self = this;
            self.editDto.Id = id;
            axios.get("../../ArquivosInventario/GetArquivosInventariosByInventarioId", {
                params: { id: id }
            })
                .then(function (response) {
                    self.arquivosInventarios = response.data;
                })
                .catch(function (error) {
                    console.error("Error fetching ArquivosInventario:", error);
                });
        },
        DeleteInventario: function (id) {
            var url = "Inventario/Delete/" + id;
            $("#deleteInventarioHref").prop("href", url);
        },
        DeleteArquivosInventario: function (id) {
            var url = "ArquivosInventario/Delete/" + id;
            $("#deleteInventarioHref").prop("href", url);
        },
        EditInventario: function (id) {
            var self = this;
            axios.get("Inventario/GetInventarioById/?id=" + id)
                .then(function (result) {
                    self.editDto.Id = result.data.id;                  
                    self.editDto.Quantidade = result.data.quantidade;
                })
                .catch(function (error) {
                    Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
                });
        },
        FilesModal: function (id) {       
            this.loadArquivosInventario(id);
            $('#mdFilesInventario').modal('show');
        }
    }
});

// Update the crud object to call the Vue instance method if needed
var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteInventarioId"]').attr('value', id);
        $('#mdDeleteInventario').modal('show');
        vm.DeleteInventario(id);
    },
    DeleteArquivosModal: function (id) {
        $('input[name="deleteArquivosInventarioId"]').attr('value', id);
        $('#mdDeleteArquivosInventario').modal('show');
        vm.DeleteArquivosInventario(id);
    },
    EditModal: function (id) {
        $('input[name="editInventarioId"]').attr('value', id);
        $('#mdEditInventario').modal('show');
        vm.EditInventario(id);
    },
    FilesModal: function (id) {
        $('input[name="editInventarioId"]').attr('value', id);
        vm.FilesModal(id);
    }
};
