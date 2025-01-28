$(document).ready(function () {
    let currentPage = 1;
    var isLoading = false;
    const randomSeed = Math.floor(Math.random() * 10000000);
    $('#seedInput').val(randomSeed);
    let table = new DataTable('#booksTable',
        {
            searching: false,
            paging: false,
            bFiltered: false,
            info: false,
            scrollCollapse: true,
            dom: 'rtip',
            buttons: ['csvHtml5'],
            "ajax": {
                "url": "/Home/GenerateBooks/",
                "type": "POST",
                dataSrc: function (data) {
                    return data.data;
                },
                "data": function (d) {
                    d.language = $('#languageDropdown').val();
                    d.seed = parseInt($('#seedInput').val(), 10) + currentPage;
                    d.likes = $('#likesSlider').val();
                    d.reviews = $('#reviewsInput').val();
                    d.numberOfBooks = 20;
                }
            },
            "columns": [
                {
                    className: 'dt-control',
                    orderable: false,
                    data: null,
                    defaultContent: ''
                },
                { "data": "id", orderable: false },
                { "data": "isbn", orderable: false, },
                { "data": "title", orderable: false, },
                {
                    "data": "authors",
                    orderable: false,
                    "render": function (data) {
                        var authorsToShow = [];
                        data.forEach(function (item) {
                            authorsToShow.push(item.firstName + " " + item.lastName);
                        });
                        return authorsToShow.join(", ");
                    }
                },
                { "data": "publisher", orderable: false }
            ],
            order: [[1, 'asc']],
        });

    table.on('click', 'td.dt-control', function (e) {
        let tr = e.target.closest('tr');
        let row = table.row(tr);
        if (row.child.isShown()) {
            row.child.hide();
        } else {
            row.child(format(row.data())).show();
        }
    });
    function format(d) {
        return `
    <div class="card flex-row" style="margin: 10px; border: 1px solid #ddd; box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);">
        <div class="flex-column" style="margin: 15px;">
            <img class="card-img-left" src="${d.imageUrl}" alt="${d.title}" style="height: 200px; width: 150px; object-fit: cover;" />
            <div style="display: flex; align-items: center; margin-top: 10px; font-size: 18px;">
                <i class="fa fa-thumbs-up" style="font-size:24px; color: red; margin-right: 5px;"></i>
                <span>${d.likes}</span>
            </div>
        </div>
        <div class="card-body" style="flex-grow: 1;">
            <h4 class="card-title">${d.title}</h4>
            <p><strong>Publisher:</strong> ${d.publisher}</p>
            <h5 class="mt-4">Reviews</h5>
            <div>
                ${d.reviews
                .map(
                    review => `
                                <blockquote class="blockquote" style="text-align: left; font-size: 0.9rem; margin-bottom: 15px;">
                                    <p class="mb-1">${review.content}</p>
                                    <footer class="blockquote-footer mt-1">${review.reviewer}</footer>
                                </blockquote>
                            `
                )
                .join('')
            }
            </div>
        </div>
    </div>
    `;
    }

    $(window).on('scroll', function () {
        if (isLoading) return;

        var viewportHeight = window.innerHeight;
        var scrolltop = $(window).scrollTop();
        var bottomOfTable = $('#booksTable').offset().top + $('#booksTable').outerHeight();

        if ($(window).scrollTop() + viewportHeight >= bottomOfTable - 200) {
            loadMoreBooks();
        }
    });

    function loadMoreBooks() {
        if (isLoading) return;
        isLoading = true;

        const lastRow = table.rows().data().toArray().slice(-1)[0];
        lastRowId = lastRow ? lastRow.id : 0;
        currentPage++;

        $.ajax({
            url: '/Home/GenerateBooks/',
            method: 'POST',
            data: {
                language: $('#languageDropdown').val(),
                seed: parseInt($('#seedInput').val(), 10) + currentPage,
                likes: $('#likesSlider').val(),
                reviews: $('#reviewsInput').val(),
                numberOfBooks: 10,
                lastRowId: lastRowId,
            },
            success: function (data) {
                data.data.forEach(book => table.row.add(book).draw(false));
                isLoading = false;
            },
            error: function () {
                console.error("Failed to load more books.");
                isLoading = false;
            }
        });
    }

    function resetAndReload() {
        currentPage = 1;
        table.ajax.reload();
    }

    $('#likesSlider').on('input', function () {
        $('#likesValue').text($(this).val());
        resetAndReload();
    });

    $('#generateSeedBtn').on('click', function () {
        const randomSeed = Math.floor(Math.random() * 10000000);
        $('#seedInput').val(randomSeed);
        resetAndReload();
    });

    $('#languageDropdown').on('change', function () {
        resetAndReload();
    });

    $('#reviewsInput').on('input', function () {
        resetAndReload();
    });

    $('#seedForm').on('submit', function (e) {
        e.preventDefault();

        const seedValue = $('#seedInput').val();
        if (!seedValue || isNaN(seedValue)) {
            alert('Please enter a valid seed!');
            return;
        }
        resetAndReload();
    });
});