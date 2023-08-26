
    $(document).ready(function () {
        loadDataTable();
    });
var dataTable;
function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": { url: '/Admin/Product/GetAll' },
        "columns": [
            { data: 'title' },
            { data: 'isbn' },
            { data: 'author' },
            { data: 'price' },
            {
                data: 'productId',
                "render": function (data) {
                    return `<a class=" link-primary text-primary" href="/Admin/Product/EditProduct?id=${data}"><i class="bi bi-pencil-square text-primary"></i> Edit </a>`                        
                }                   
            },
            {
                data: 'productId',
                "render": function (data) {
                    return `<a onClick= Delete("/Admin/Product/DeleteProduct?id=${data}")> <i class="bi bi-trash-fill text-danger"></i> </a>`
                }
            }

        ]
    })
}
function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax( {
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    Swal.fire(
                       data.message
                    )
                }
            })           
        }
    })

}
